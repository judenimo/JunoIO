//! Four UDP sockets matching the Python reference: two bound for inbound
//! traffic, two connected for outbound traffic.

use std::io;
use std::net::SocketAddr;

use tokio::net::UdpSocket;

use crate::error::Result;

pub const DEFAULT_HOST: &str = "127.0.0.1";
pub const DEFAULT_TELEMETRY_PORT: u16 = 5005;
pub const DEFAULT_COMMAND_PORT: u16 = 5006;
pub const DEFAULT_RPC_REQUEST_PORT: u16 = 5007;
pub const DEFAULT_RPC_RESPONSE_PORT: u16 = 5008;

#[derive(Debug, Clone, Copy)]
pub struct Addresses {
    pub telemetry: SocketAddr,
    pub command: SocketAddr,
    pub rpc_request: SocketAddr,
    pub rpc_response: SocketAddr,
}

impl Default for Addresses {
    fn default() -> Self {
        let host = DEFAULT_HOST;
        Self {
            telemetry: format!("{host}:{DEFAULT_TELEMETRY_PORT}").parse().unwrap(),
            command: format!("{host}:{DEFAULT_COMMAND_PORT}").parse().unwrap(),
            rpc_request: format!("{host}:{DEFAULT_RPC_REQUEST_PORT}").parse().unwrap(),
            rpc_response: format!("{host}:{DEFAULT_RPC_RESPONSE_PORT}").parse().unwrap(),
        }
    }
}

pub(crate) struct UdpBridge {
    telemetry: UdpSocket,
    command: UdpSocket,
    rpc_request: UdpSocket,
    rpc_response: UdpSocket,
}

impl UdpBridge {
    pub async fn bind(addrs: Addresses) -> Result<Self> {
        let telemetry = UdpSocket::bind(addrs.telemetry).await?;

        let command = UdpSocket::bind("0.0.0.0:0").await?;
        command.connect(addrs.command).await?;

        let rpc_response = UdpSocket::bind(addrs.rpc_response).await?;

        let rpc_request = UdpSocket::bind("0.0.0.0:0").await?;
        rpc_request.connect(addrs.rpc_request).await?;

        Ok(Self {
            telemetry,
            command,
            rpc_request,
            rpc_response,
        })
    }

    /// Await at least one telemetry datagram, then drain any backlog and
    /// return only the freshest packet. Mirrors `receive_latest_snapshot`
    /// in transport.py.
    pub async fn recv_latest_telemetry(&self) -> Result<Vec<u8>> {
        let mut buf = vec![0u8; 65535];
        let n = self.telemetry.recv(&mut buf).await?;
        let mut latest = buf[..n].to_vec();

        loop {
            match self.telemetry.try_recv(&mut buf) {
                Ok(m) => {
                    latest.clear();
                    latest.extend_from_slice(&buf[..m]);
                }
                Err(e) if e.kind() == io::ErrorKind::WouldBlock => break,
                Err(e) => return Err(e.into()),
            }
        }

        Ok(latest)
    }

    pub async fn send_command(&self, bytes: &[u8]) -> Result<()> {
        self.command.send(bytes).await?;
        Ok(())
    }

    pub async fn send_rpc_request(&self, bytes: &[u8]) -> Result<()> {
        self.rpc_request.send(bytes).await?;
        Ok(())
    }

    pub async fn recv_rpc_response(&self) -> Result<Vec<u8>> {
        let mut buf = vec![0u8; 65535];
        let n = self.rpc_response.recv(&mut buf).await?;
        buf.truncate(n);
        Ok(buf)
    }
}
