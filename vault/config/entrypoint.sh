#!/bin/sh

set -e

# Start Vault server in dev mode in the background
vault server -dev -dev-listen-address="0.0.0.0:8200" &
sleep 5s

vault secrets enable database

# This container is now healthy
touch /tmp/healthy

# Keep container alive
tail -f /dev/null & trap 'kill %1' TERM ; wait
