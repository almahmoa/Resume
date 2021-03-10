# Author: Abraham Almahmoud
# Date: 02/17/2020
# Description: Write a LinkedList class that has recursive implementations of the add, display, and remove methods
#              described in the lesson. It should also have recursive implementations of the contains, insert, and
#              reverse methods described in the exercises. You may use default arguments and/or helper functions.


class Node:
    """
    Represents a node in a linked list
    """
    def __init__(self, data):
        self.data = data
        self.next = None


class LinkedList:
    """
    A linked list implementation of the List ADT
    """
    def __init__(self):
        self.head = None

    def add(self, val):
        """
        Adds a node containing val to the linked list
        """
        if self.head is None:  # If the list is empty
            self.head = Node(val)
            return

        def add_recursive(current):
            """
            Uses recursion to cycle down current until next is None to add node
            """
            if current.next is not None:  # if the coming value is not None
                return add_recursive(current.next)
            current.next = Node(val)  # add the node at the end of the link list
        return add_recursive(current=self.head)  # initiate the recursion

    def display(self):
        """
        Prints out the values in the linked list
        """
        def display_recursive(current):
            """
            Cycle down the link list, and print the nodes
            """
            if current is not None:
                print(current.data, end=" ")  # add the the print list as long as current is not empty
                return display_recursive(current.next)
            print()  # output final list
        return display_recursive(current=self.head)  # initiate the recursion

    def remove(self, val):
        """
        Removes the node containing val from the linked list
        """
        if self.head is None:  # If the list is empty
            return
        if self.head.data == val:  # If the node to remove is the head
            self.head = self.head.next
            return

        def remove_recursive(current, previous):
            """
            cycle down the link list to find the val, and remove it
            """
            if current is not None and current.data != val:
                return remove_recursive(current.next, current)
            if current is not None:
                previous.next = current.next
        return remove_recursive(current=self.head, previous=self.head)  # initiate the recursion

    def contains(self, val):
        """
        Use the value in the parameter and returns True if that value is in the linked list,
        returns False otherwise.
        """
        if self.head is None:
            return

        def contains_recursive(current):
            """
            cycles current to find the value
            """
            if current is not None:
                if current.data == val:
                    return True
                return contains_recursive(current.next)
            return False
        return contains_recursive(current=self.head)  # initiate the recursion

    def insert(self, val, pos):
        """
        Places a node of val, in the position entered
        """
        if self.head is None:
            return
        if pos == 0:  # check if the insert is at the head
            temp = self.head
            self.head = Node(val)
            self.head.next = temp
            return

        def insert_recursive(current, counter):
            """
            Cycles the link list until position is reached,
            or end of list is reached
            """
            if counter != pos-1:  # check if counter is the same value of the position(-1 for placement)
                if current.next is None:  # if next value is empty, insert node of val
                    current.next = Node(val)
                    return
                return insert_recursive(current.next, counter+1)  # move current, and counter down
            _temp = current.next
            current.next = Node(val)
            current.next.next = _temp
        return insert_recursive(current=self.head, counter=0)  # initiate the recursion

    def reverse(self):
        """
        initiates the reverse recursive function by setting the self.head
        """
        def reverse_recursive(current, previous):
            """
            cycles down the link list, reversing the order
            """
            if current is None:  # check if current has value
                return previous  # if current is None, set self.head to previous
            temp = current.next
            current.next = previous
            previous = current
            current = temp
            return reverse_recursive(current, previous)
        self.head = reverse_recursive(current=self.head, previous=None)  # initiate the recursion

    def is_empty(self):
        """
        Returns True if the linked list is empty,
        returns False otherwise
        """
        return self.head is None
