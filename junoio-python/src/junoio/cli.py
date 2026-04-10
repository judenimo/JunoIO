from __future__ import annotations

import argparse
import socket
from pathlib import Path

from .runtime import JunoRuntime
from .transport import DEFAULT_HOST, DEFAULT_TELEMETRY_PORT


def build_parser() -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(prog="junoio")
    subparsers = parser.add_subparsers(dest="command", required=True)

    run_parser = subparsers.add_parser("run", help="Run a Python flight-computer script.")
    run_parser.add_argument("script", type=Path, help="Path to the user script that defines loop().")

    listen_parser = subparsers.add_parser("listen", help="Listen for raw telemetry packets.")
    listen_parser.add_argument(
        "--host",
        default=DEFAULT_HOST,
        help=f"Host to bind for telemetry listening. Defaults to {DEFAULT_HOST}.",
    )
    listen_parser.add_argument(
        "--port",
        type=int,
        default=DEFAULT_TELEMETRY_PORT,
        help=f"Port to bind for telemetry listening. Defaults to {DEFAULT_TELEMETRY_PORT}.",
    )
    listen_parser.add_argument(
        "--count",
        type=int,
        default=0,
        help="Number of packets to print before exiting. Defaults to 0 for infinite.",
    )

    return parser


def listen_for_packets(host: str, port: int, count: int) -> int:
    packet_limit = max(count, 0)
    printed = 0

    with socket.socket(socket.AF_INET, socket.SOCK_DGRAM) as sock:
        sock.bind((host, port))
        print(f"Listening on {host}:{port}")

        while packet_limit == 0 or printed < packet_limit:
            payload, _ = sock.recvfrom(65535)
            print(payload.decode("utf-8"))
            printed += 1

    return 0


def main(argv: list[str] | None = None) -> int:
    parser = build_parser()
    args = parser.parse_args(argv)

    if args.command == "listen":
        return listen_for_packets(args.host, args.port, args.count)

    if args.command != "run":
        parser.error(f"Unsupported command: {args.command}")

    runtime = JunoRuntime()
    update_callback = runtime.load_user_script(args.script)

    try:
        runtime.run(update_callback)
    except KeyboardInterrupt:
        return 0

    return 0
