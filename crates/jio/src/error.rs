use thiserror::Error;

#[derive(Debug, Error)]
pub enum Error {
    #[error("io: {0}")]
    Io(#[from] std::io::Error),
    #[error("json: {0}")]
    Json(#[from] serde_json::Error),
    #[error("rpc error: {0}")]
    Rpc(String),
    #[error("rpc timed out: {0}")]
    RpcTimeout(String),
}

pub type Result<T> = std::result::Result<T, Error>;
