#!/bin/sh

cd /home/necrommunity/Statsbot
python3 client.py >> /home/necrommunity/Statsbot/log/output-$(date +"%Y-%m-%d").log
