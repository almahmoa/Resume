# student_list.py
# Author: Abraham Almahmoud
# Date: 04/21/2020
# Description: Reimplementation of Pythons List
# ===================================================

import numpy as np


# StudentList class is our implementation of Python's List
class StudentList:
    def __init__(self):
        # creates an empty array of length 4, change the [4] to another value to make an
        # array of different length.
        self._list = np.empty([4], np.int16)
        self._capacity = 4
        self._size = 0

    def __str__(self):
        return str(self._list[:self._size])

# You may want an internal function that handles resizing the array.
# Dont modify get_list or get_capacity, they are for testing

    def get_list(self):
        return self._list[:self._size]

    def get_capacity(self):
        return self._capacity

    def _upsize(self):  # double capacity size if full
        if self._capacity == 0:
            self._capacity = 1
        new_data = np.empty([self._capacity * 2], np.int16)
        self._list = np.append(self._list, new_data)
        self._capacity = self._capacity * 2

    def append(self, val):  # add at the end of the stack
        if self._capacity == self._size:
            self._upsize()
        if type(val) is list:   # check if val was a list
            if self._capacity <= self._size + len(val):
                self._upsize()
                self.append(val)
            for item in val:
                self._list[self._size] = item
                self._size = self._size + 1
        else:   # val is a single element
            self._list[self._size] = val
            self._size = self._size + 1

    def pop(self):  # remove last value from stack
        if self._size == 0:
            return
        self._size = self._size - 1
        return self._list[self._size]

    def insert(self, index, val):   # add value at a certain location in the stack
        if index > self._size:  # check if index is out of range
            return
        if self._capacity == self._size:
            self._upsize()
        self._list = np.insert(self._list, index, val)
        self._size = self._size + 1

    def remove(self, val):  # remove a value at a certain location
        if self._size == 0 or val-1 > self._size:
            return
        self._list = np.delete(self._list, val-1)
        self._size = self._size - 1

    def clear(self):    # empty the stack
        self._list = np.empty([0], np.int16)
        self._size = 0
        self._capacity = 0

    def count(self):    # count the total cost of stack
        return np.count_nonzero(self._list)

    def get(self, index):   # return the value at a specific location
        return self._list[index]
