#! /usr/bin/env python

import yaml
import copy
import re
import random

biomesPath = "StreamingAssets/worldgen/Nubrion/biomes"
subworldsPath = "StreamingAssets/worldgen/Nubrion/subworlds"

with open("D:/SteamLibrary/steamapps/common/OxygenNotIncluded/OxygenNotIncluded_Data/StreamingAssets/worldgen/temperatures.yaml", 'r') as stream:
  temperatures = yaml.load(stream)

with open("BandsTemplate.yaml", 'r') as stream:
  template = yaml.load(stream)

zoneTypes = ["BoggyMarsh", "FrozenWastes", "OilField", "Sandstone", "ToxicJungle"] # "Space"

def readCells(filename):
  with open(filename, 'r') as stream:
    data = yaml.load(stream)
    
  if isinstance(data, dict):
    cells = data['cells']

    cell_dict = dict()
    for x in cells:
      cell_dict[x['element']] = x

    cells = list(cell_dict.values())
  elif isinstance(data, list):
    cells = data
  else:
    cells = [data]

  return cells

def writeFile(data, filename):
  print("Writing " + filename)
  with open(filename, 'w') as stream:
    yaml.dump(data, stream, default_flow_style=False)

def getTemperatureRange(x):
  global temperatures
  
  t = x['temperature']
  
  for name,range in temperatures['ranges'].items():
    if t >= range['min'] and t <= range['max']:
      return name
  
  if t > 1500:
    return "ExtremelyHot"
  return "ExtremelyCold"
  
def makeBands(terrainBiomeLookupTable, cells):
  for x in cells:
    tempRange = getTemperatureRange(x)
    
    if tempRange not in terrainBiomeLookupTable:
      terrainBiomeLookupTable[tempRange] = list()
    
    Basic = terrainBiomeLookupTable[tempRange]
    
    band = dict()
    band['content'] = x['element']
    band['bandSize'] = 2
    
    Basic.append(band)

def makeSubworld(tempRange):
  global template
  global zoneTypes
  global subworldsPath
  
  subworld = copy.deepcopy(template)
  
  biome = dict()
  biome['name'] = "Nubrion/biomes/TemperatureBands/" + tempRange
  biome['weight'] = 1
  
  subworld['name'] = "Band" + tempRange
  subworld['zone']['temperatureRange'] = tempRange
  subworld['zone']['biomes'] = [biome]
  subworld['zone']['zoneType'] = random.choice(zoneTypes)
  
  writeFile(subworld, subworldsPath + "/" + subworld['name'] + ".yaml" )

#cells.reverse()

materials = readCells("all_temperature_materials.yaml")

object = dict()
terrainBiomeLookupTable = dict()
object['TerrainBiomeLookupTable'] = terrainBiomeLookupTable
makeBands(terrainBiomeLookupTable, materials)
writeFile(object, biomesPath + "/TemperatureBands.yaml")

for name, range in terrainBiomeLookupTable.items():
  makeSubworld(name)
