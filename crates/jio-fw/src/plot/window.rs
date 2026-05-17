use egui::CentralPanel;

use super::group::PlotGroup;

struct PlotApp {
    group: PlotGroup,
}

impl eframe::App for PlotApp {
    fn update(&mut self, ctx: &egui::Context, _frame: &mut eframe::Frame) {
        ctx.request_repaint();
        CentralPanel::default().show(ctx, |ui| {
            self.group.show(ui);
        });
    }
}

/// Spawn `sim` on a background thread, then open a blocking eframe window
/// that renders `group` until the window is closed.
pub fn run_plot_window<F>(title: &str, group: PlotGroup, sim: F)
where
    F: FnOnce() + Send + 'static,
{
    std::thread::spawn(sim);
    eframe::run_native(
        title,
        eframe::NativeOptions::default(),
        Box::new(|_cc| Ok(Box::new(PlotApp { group }))),
    )
    .unwrap();
}
