#!/bin/sh

set -e

# Start Vault server in dev mode in the background
vault server -dev -dev-listen-address="0.0.0.0:8200" &
sleep 5s

export VAULT_ADDR='http://0.0.0.0:8200'

vault secrets enable database

vault write database/config/postgresql \
     plugin_name=postgresql-database-plugin \
     connection_url="postgresql://{{username}}:{{password}}@smaiax-backend-db/smaiax-db?sslmode=disable" \
     allowed_roles=readonly \
     username="user" \
     password="password"

# This container is now healthy
touch /tmp/healthy

# Keep container alive
tail -f /dev/null & trap 'kill %1' TERM ; wait
