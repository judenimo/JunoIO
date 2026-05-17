use std::collections::HashMap;

use egui_plot::{Line, Plot, PlotPoints};

use super::channel::Channel;

// ── Bound ────────────────────────────────────────────────────────────────────

pub(crate) enum Bound {
    Fixed(f64),
    /// Receives the current data-min or data-max of the subplot, returns the bound.
    Fn(Box<dyn Fn(f64) -> f64 + Send + Sync>),
}

impl Bound {
    fn resolve(&self, data_extreme: f64) -> f64 {
        match self {
            Bound::Fixed(v) => *v,
            Bound::Fn(f) => f(data_extreme),
        }
    }
}

/// Accepts either a fixed `f64` or a closure `|data_extreme: f64| -> f64`.
pub(crate) trait IntoBound: sealed::Sealed {
    #[doc(hidden)]
    fn into_bound(self) -> Bound;
}

mod sealed {
    pub trait Sealed {}
    impl Sealed for f64 {}
    impl<F: Fn(f64) -> f64 + Send + Sync + 'static> Sealed for F {}
}

impl IntoBound for f64 {
    fn into_bound(self) -> Bound { Bound::Fixed(self) }
}

impl<F: Fn(f64) -> f64 + Send + Sync + 'static> IntoBound for F {
    fn into_bound(self) -> Bound { Bound::Fn(Box::new(self)) }
}

// ── SubplotSpec ───────────────────────────────────────────────────────────────

struct SubplotSpec {
    title: String,
    xlabel: Option<String>,
    ylabel: Option<String>,
    channels: Vec<Channel>,
    ymin: Option<Bound>,
    ymax: Option<Bound>,
}

// ── PlotGroup ─────────────────────────────────────────────────────────────────

pub struct PlotGroup {
    channels: HashMap<String, Channel>,
    subplots: Vec<SubplotSpec>,
    capacity: usize,
}

impl PlotGroup {
    pub fn new(capacity: usize) -> Self {
        Self { channels: HashMap::new(), subplots: vec![], capacity }
    }

    /// Register a named streaming channel in the global registry.
    pub fn channel(&mut self, label: impl Into<String>) {
        let label = label.into();
        self.channels.insert(label.clone(), Channel::new(label, self.capacity));
    }

    /// Install all registered channels into the global registry.
    /// Call once before `run_plot_window` so `jio_fw::plot::push` works from any thread.
    pub fn install(&self) {
        super::set_global(
            self.channels.iter().map(|(k, v)| (k.clone(), v.clone())).collect(),
        );
    }

    /// Declare a subplot and attach channels to it via the returned builder.
    pub fn subplot(&mut self, title: impl Into<String>) -> PlotBuilder<'_> {
        self.subplots.push(SubplotSpec {
            title: title.into(),
            xlabel: None,
            ylabel: None,
            channels: vec![],
            ymin: None,
            ymax: None,
        });
        PlotBuilder { group: self }
    }

    /// Render all subplots in a responsive grid. Call every frame from `eframe::App::update`.
    pub fn show(&self, ui: &mut egui::Ui) {
        let n = self.subplots.len();
        if n == 0 { return; }

        let spacing = 4.0_f32;
        let min_plot_w = 400.0_f32;
        let plot_h = 300.0_f32;

        let avail_w = ui.available_width();
        let cols = (((avail_w + spacing) / (min_plot_w + spacing)).floor() as usize)
            .max(1)
            .min(n);
        let plot_w = (avail_w - spacing * (cols as f32 - 1.0)) / cols as f32;

        egui::ScrollArea::vertical().show(ui, |ui| {
            for (row_idx, chunk) in self.subplots.chunks(cols).enumerate() {
                if row_idx > 0 {
                    ui.add_space(spacing);
                }
                ui.horizontal(|ui| {
                    ui.spacing_mut().item_spacing.x = spacing;
                    for spec in chunk {
                        let mut plot = Plot::new(&spec.title).width(plot_w).height(plot_h);
                        if let Some(xl) = &spec.xlabel { plot = plot.x_axis_label(xl); }
                        if let Some(yl) = &spec.ylabel { plot = plot.y_axis_label(yl); }

                        let snapshots: Vec<Vec<[f64; 2]>> =
                            spec.channels.iter().map(|ch| ch.snapshot()).collect();

                        if spec.ymin.is_some() || spec.ymax.is_some() {
                            let all_y = snapshots.iter().flatten().map(|p| p[1]);
                            let data_min = all_y.clone().fold(f64::INFINITY, f64::min);
                            let data_max = all_y.fold(f64::NEG_INFINITY, f64::max);

                            if let Some(b) = &spec.ymin {
                                if data_min.is_finite() || matches!(b, Bound::Fixed(_)) {
                                    plot = plot.include_y(b.resolve(data_min));
                                }
                            }
                            if let Some(b) = &spec.ymax {
                                if data_max.is_finite() || matches!(b, Bound::Fixed(_)) {
                                    plot = plot.include_y(b.resolve(data_max));
                                }
                            }
                        }

                        plot.show(ui, |pui| {
                            for (ch, pts) in spec.channels.iter().zip(snapshots) {
                                pui.line(Line::new(PlotPoints::new(pts)).name(ch.label()));
                            }
                        });
                    }
                });
            }
        });
    }
}

// ── PlotBuilder ───────────────────────────────────────────────────────────────

pub struct PlotBuilder<'a> {
    group: &'a mut PlotGroup,
}

impl<'a> PlotBuilder<'a> {
    /// Attach a registered channel by name to this subplot.
    pub fn line(self, name: &str) -> Self {
        let ch = self.group.channels.get(name)
            .unwrap_or_else(|| panic!("unknown channel: {name}"))
            .clone();
        self.group.subplots.last_mut().unwrap().channels.push(ch);
        self
    }

    pub fn xlabel(self, label: impl Into<String>) -> Self {
        self.group.subplots.last_mut().unwrap().xlabel = Some(label.into());
        self
    }

    pub fn ylabel(self, label: impl Into<String>) -> Self {
        self.group.subplots.last_mut().unwrap().ylabel = Some(label.into());
        self
    }

    /// Set the y-axis lower bound. Accepts a fixed `f64` or a closure `|data_min| data_min - 5.0`.
    #[allow(private_bounds)]
    pub fn ymin(self, bound: impl IntoBound) -> Self {
        self.group.subplots.last_mut().unwrap().ymin = Some(bound.into_bound());
        self
    }

    /// Set the y-axis upper bound. Accepts a fixed `f64` or a closure `|data_max| data_max + 10.0`.
    #[allow(private_bounds)]
    pub fn ymax(self, bound: impl IntoBound) -> Self {
        self.group.subplots.last_mut().unwrap().ymax = Some(bound.into_bound());
        self
    }
}
