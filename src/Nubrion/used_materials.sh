#! /bin/sh

grep -hr 'content:' StreamingAssets/worldgen/*/biomes/ | sed 's/.*://' | sort | uniq
