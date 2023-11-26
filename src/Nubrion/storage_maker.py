#! /usr/bin/env python

import yaml
import copy
import re

with open("all_materials.yaml", 'r') as stream:
  data = yaml.load(stream)
with open("element_template.yaml", 'r') as stream:
  template = yaml.load(stream)
  
out_path = "StreamingAssets/templates/features"

cells = data['cells']

cell_dict = dict()
for x in cells:
  cell_dict[x['element']] = x

cells = list(cell_dict.values())

for x in cells:
  object = copy.deepcopy(template)
  
  object['elementalOres'] = list()
  for cellTemplate in template['elementalOres']:
    cell = copy.deepcopy(cellTemplate)
  
    if cell['id'] == 'LiquidHydrogen':
      cell['id'] = x['element']
      cell['element'] = x['element']
      cell['temperature'] = x['temperature']
      
    while cell['units'] > 20000:
      cellSplit = copy.deepcopy(cell)
      cellSplit['units'] = 20000
      cell['units'] = cell['units'] - cellSplit['units']
      object['elementalOres'].append(cellSplit)
    
    if cell['units'] > 0:
      object['elementalOres'].append(cell)
  
  if x['element'] == 'Unobtanium':
    for cell in object['cells']:
      if cell['element'] != 'LiquidHydrogen':
        continue
      cell['element'] = 'Vacuum'
      cell['temperature'] = 0
      cell['mass'] = 0
  else:
    for cell in object['cells']:
      if cell['element'] != 'LiquidHydrogen':
        continue
      
      cell['element'] = x['element']
      cell['temperature'] = x['temperature']
      cell['mass'] = x['mass']
  
  name = re.sub('(.)([A-Z][a-z]+)', r'\1_\2', x['element'])
  name = re.sub('([a-z0-9])([A-Z])', r'\1_\2', name).lower()
  name = "safe_storage_" + name
  
  object['name'] = name
  
  filename = out_path + "/" + object['name'] + ".yaml"
  print("Writing " + filename)
  with open(filename, 'w') as stream:
    yaml.dump(object, stream, default_flow_style=False)
