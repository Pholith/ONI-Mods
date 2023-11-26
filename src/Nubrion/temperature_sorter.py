#! /usr/bin/env python

import yaml

name = "solid"

with open("all_" + name +".yaml", 'r') as stream:
  data = yaml.load(stream)

cells = data['cells']

cell_dict = dict()
for x in cells:
  x.pop('diseaseCount', None)
  x.pop('diseaseName', None)
  x.pop('location_x', None)
  x.pop('location_y', None)
  cell_dict[x['element']] = x

cells = list(cell_dict.values())

cells.sort(key=lambda x: x['temperature'] * 1000000.0 + x['mass'])

with open("all_temperature_" + name + ".yaml", 'w') as stream:
  yaml.dump(cells, stream, default_flow_style=False)
