# word_count.py
# Author: Abraham Almahmoud
# Date: 06/07/2020
# ===================================================
# Implement a word counter that counts the number of
# occurrences of all the words in a file. The word
# counter will return the top X words, as indicated
# by the user.
# ===================================================

import re
from hash_map import HashMap

"""
This is the regular expression used to capture words. It could probably be endlessly
tweaked to catch more words, but this provides a standard we can test against, so don't
modify it for your assignment submission.
"""
rgx = re.compile("(\w[\w']*\w|\w)")


def second_value(element):
    return element[1]


def hash_function_2(key):
    """
    This is a hash function that can be used for the hashmap.
    """

    hash = 0
    index = 0
    for i in key:
        hash = hash + (index + 1) * ord(i)
        index = index + 1
    return hash


def top_words(source, number):
    """
    Takes a plain text file and counts the number of occurrences of case insensitive words.
    Returns the top `number` of words in a list of tuples of the form (word, count).

    Args:
        source: the file name containing the text
        number: the number of top results to return (e.g. 5 would return the 5 most common words)
    Returns:
        A list of tuples of the form (word, count), sorted by most common word. (e.g. [("a", 23), ("the", 20), ("it", 10)])
    """

    keys = set()
    ht = HashMap(2500, hash_function_2)
    tups = []

    # This block of code will read a file one word as a time and
    # put the word in `w`. It should be left as starter code.
    with open(source) as f:
        for line in f:
            words = rgx.findall(line)
            for w in words:
                num = 1
                w = w.lower()
                if ht.contains_key(w):  # check if word is already the hash map
                    num = ht.get(w)     # store count in existing word hash
                    num += 1
                ht.put(w, num)          # run put function to place word in hash map
    for buckets in ht.resize_table(1):  # link all tuples together in a single list.
        cur = buckets.head
        while cur:
            k = cur.key
            v = cur.value
            keys.add((k, v))            # create a list from the values in the hash map
            temp = cur
            cur = cur.next
            temp.next = cur
    keys = sorted(keys, key=second_value, reverse=True)
    for top in range(0, number):        # store the range of tuples in a new list
        tups.append(keys[top])
    return tups

# COMMENT THIS OUT WHEN SUBMITTING TO GRADESCOPE
# print(top_words("alice.txt", 10))
