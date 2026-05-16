{
  description = "EGui Flake";

  inputs = {
    nixpkgs.url = "github:NixOS/nixpkgs/nixos-unstable";
    flake-utils.url = "github:numtide/flake-utils";
  };

  outputs =
    {
      self,
      nixpkgs,
      flake-utils,
    }:
    flake-utils.lib.eachDefaultSystem (
      system:
      let
        pkgs = nixpkgs.legacyPackages.${system};
      in
      {
        devShells.default = pkgs.mkShell rec {
          name = "egui";
          nativeBuildInputs = with pkgs; [
            pkg-config
            libxkbcommon
            wayland
            libGL
          ];

          buildInputs = with pkgs; [
            wayland
            pkg-config
            openssl
            pango

            cmake
            ninja
            ccache
            udev

            libclang
            linuxHeaders

            libGL
            libxkbcommon
            vulkan-loader
            glibc.dev
            atk
            gdk-pixbuf
            gobject-introspection
            cairo
            gtk3
            glib
          ];

          shellHook = ''
            export LD_LIBRARY_PATH="$LD_LIBRARY_PATH:${
              pkgs.lib.makeLibraryPath [
                pkgs.wayland
                pkgs.libxkbcommon
              ]
            }"

                    export BINDGEN_EXTRA_CLANG_ARGS="\
                      -isystem ${pkgs.linuxHeaders}/include \
                      -isystem ${pkgs.glibc.dev}/include"
          '';
        };
      }
    );
}
