# Author: Abraham Almahmoud
# Date: 02/02/2020
# Description: Write a class named NeighborhoodPets that has methods for adding a pet, deleting a pet, searching for
#              the owner of a pet, saving data to a JSON file, loading data from a JSON file, and getting a list of all
#              pet species.  It will only be loading JSON files that it has previously created, so the internal
#              organization of the data is up to you.

import json


class NeighborhoodPets:
    """create and save pet objects in json files"""
    def __init__(self, pets=[]):
        """create a base json file to start with"""
        with open("main_pets.json", 'w') as outfile:
            json.dump(pets, outfile)

        with open("main_pets.json", 'r') as infile:
            self.pet_file = json.load(infile)
            self.pet_file_name = "main_pets.json"  # set name of json to the base json file

    def add_pet(self, p_name, p_species, o_name):
        """create a new pet using the parameters passed. if pet has same name, return without adding the new pet"""
        for pets in range(0, len(self.pet_file)):
            if self.pet_file[pets]["pet name"] == p_name:
                return None  # duplicate pet name was found
        self.pet_file.append({"pet name": p_name, "pet species": p_species, "owner name": o_name})  # add new pet dict
        with open(self.pet_file_name, 'w') as outfile:
            json.dump(self.pet_file, outfile)  # confirm changes in json file

    def delete_pet(self, p_name):
        """takes as a parameter the name of the pet and deletes it"""
        for pets in range(0, len(self.pet_file)):
            if self.pet_file[pets]["pet name"] == p_name:
                self.pet_file.pop(pets)  # remove requested pet dict
                break  # leave 'for loop' to prevent out of index error
        with open(self.pet_file_name, 'w') as outfile:
            json.dump(self.pet_file, outfile)  # confirm changes in json file

    def get_owner(self, p_name):
        """takes as a parameter the name of the pet and returns the name of its owner"""
        for pets in range(0, len(self.pet_file)):
            if self.pet_file[pets]["pet name"] == p_name:
                return self.pet_file[pets]["owner name"]  # return owner's name based on pet's name
        return None  # no pet was found with that name

    def save_as_json(self, json_name):
        """takes as a parameter the name of the file and saves it in json format with that name"""
        with open(json_name, "w") as outfile:
            json.dump(self.pet_file, outfile)  # save create json file with current information
            self.pet_file_name = json_name  # set name to passed name

    def read_json(self, json_name):
        """takes as a parameter the name of the file to read and loads that file.
        this will replace the pets currently in memory."""
        with open(json_name, 'r') as infile:
            self.pet_file = json.load(infile)  # load existing json file
            self.pet_file_name = json_name  # set name to passed name

    def get_all_species(self):
        """takes no parameters and returns a set of the species of all pets"""
        species = []  # empty list to store
        for pets in range(0, len(self.pet_file)):
            species.append(self.pet_file[pets]["pet species"])  # populate species list with species value
        return species
