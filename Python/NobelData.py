# Author: Abraham Almahmoud
# Date: 02/02/2020
# Description: Write a class named NobelData that reads a JSON file containing data on Nobel Prizes and allows the user
#              to search that data. It just needs to read a local JSON file - it doesn't need to access the internet.
#              Specifically, your class should have an init method that reads the file, and it should have a method
#              named search_nobel that takes as parameters a year and a category, and returns a sorted list (in normal
#              dictionary order) of the surnames for the winner(s) in that category for that year (up to three people
#              can share the prize). The year will be a string (e.g. "1975"), not a number. The categories are:
#              "chemistry", "economics", "literature", "peace", "physics", and "medicine". The JSON file will be named
#              nobels.json and will be provided - you do not need to submit it.

import json


class NobelData:
    """read json file containing data on nobel prizes, and search for winners based on year/category"""
    def __init__(self):
        """open the json file containing the data on noble prizes winners"""
        with open("nobels.json", 'r') as infile:
            self.nobels = json.load(infile)

    def search_nobel(self, year, category):
        """go through the json file using the passed values to find the winners for the values"""
        awarded = []
        nobel = self.nobels["prizes"]
        for prizes in range(len(nobel)):
            if nobel[prizes]["year"] == year:  # find year that match
                if nobel[prizes]["category"] == category:  # find category that match
                    for winners in range(0, len(nobel[prizes]["laureates"])):  # store values 1-3
                        awarded.append(nobel[prizes]["laureates"][winners]["surname"])
        awarded.sort()  # sort name based on dictionary order
        return awarded
