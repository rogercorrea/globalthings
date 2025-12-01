#!/bin/bash
set -e

echo "[Kafka init] Waiting for Kafka broker..."
cub kafka-ready -b localhost:9092 1 20

echo "[Kafka init] Creating topics..."

kafka-topics --create --topic sensors.measurements \
  --bootstrap-server localhost:9092 \
  --partitions 1 \
  --replication-factor 1 \
  --if-not-exists

echo "[Kafka init] Topics created successfully."
