# linked_list.py
# Author: Abraham Almahmoud
# Date: 05/04/2020
# Description: implement Singly Linked List and Doubly Link List
# ===================================================
# Linked list exploration
# Part 1: implement the deque and bag ADT with a Linked List
# Part 2: implement the deque ADT with a CircularlyDoubly-
# Linked List
# ===================================================


'''
**********************************************************************************
Part1: Deque and Bag implemented with Linked List
**********************************************************************************
'''


class SLNode:
    def __init__(self):
        self.next = None
        self.data = None


class LinkedList:
    def __init__(self, start_list=None):
        """
        Initializes a linked list with a head and tail node with None data
        """
        self.head = SLNode()
        self.tail = SLNode()
        self.head.next = self.tail

        # populate list with initial set of nodes (if provided)
        if start_list is not None:
            for data in start_list:
                self.add_back(data)

    def __str__(self):
        """
        Returns a human readable string of the list content of the form
        [value1 -> value2 -> value3]

        An empty list should just return []

        Returns:
            The string of the human readable list representation
        """
        out = '['
        if self.head.next != self.tail:             
            cur = self.head.next.next
            out = out + str(self.head.next.data)
            while cur != self.tail:
                out = out + ' -> ' + str(cur.data)
                cur = cur.next
        out = out + ']'
        return out

    def add_link_before(self, data, index):
        """
        Adds a new link containing data and inserts it before the link at index.
        If index is 0, it inserts at the beginning of the list.

        Args:
            data: The data the new node will contain
            index: The index of the node that will immediately follow the newly added node
        """
        new_link = SLNode()  # initialize a new link
        new_link.data = data  # set new_link data
        if self.head is None:
            return
        if index == 0:  # check if the insert is at the head
            temp = self.head.next
            self.head.next = new_link
            self.head.next.next = temp
            return

        def insert_recursive(current, counter):
            """
            Cycles the link list until position is reached,
            or end of list is reached
            """
            if counter != index:  # check if counter is the same value of the position(-1 for placement)
                if current.next is None:  # if next value is empty, insert node of val
                    current.next = new_link
                    return
                return insert_recursive(current.next, counter + 1)  # move current, and counter down
            _temp = current.next
            current.next = new_link
            current.next.next = _temp

        return insert_recursive(current=self.head, counter=0)  # initiate the recursion

    def remove_link(self, index):
        """
        Removes the link at the location specified by index
        Args:
            Index: The index of the node that will be removed
        """
        if self.head is None:  # If the list is empty
            return
        if index == 0:  # If the node to remove is the head
            self.head = self.head.next
            return

        def remove_recursive(current, previous, counter):
            """
            cycle down the link list to find the index, and remove it
            """
            if current is not None and counter != index + 1:
                return remove_recursive(current.next, current, counter + 1)
            if current is not None:
                previous.next = current.next

        return remove_recursive(current=self.head, previous=self.head, counter=0)  # initiate the recursion

    def add_front(self, data):
        """
        Adds a new node after the head that contains data

        Args:
            data: The data the new node will contain
        """
        new_link = SLNode()  # initialize a new link
        new_link.data = data  # set new_link data

        temp = self.head.next
        self.head.next = new_link
        self.head.next.next = temp

    def add_back(self, data):
        """
        Adds a new node before the tail that contains data

        Args:
            data: The data the new node will contain
        """
        new_link = SLNode()  # initialize a new link
        new_link.data = data  # set new_link data

        if self.head is None:
            self.head = new_link
            return

        def add_back_recursive(current):
            """
            Uses recursion to cycle down current until next is None to add node
            """
            if current.next.next is not None:  # if the coming value is not None
                return add_back_recursive(current.next)
            _temp = current.next
            current.next = new_link
            current.next.next = _temp

        return add_back_recursive(current=self.head)  # initiate the recursion

    def get_front(self):
        """
        Returns the data in the element at the front of the list. Will return
        None in an empty list.

        Returns:
            The data in the node at index 0 or None if there is no such node
        """
        return self.head.next.data

    def get_back(self):
        """
        Returns the data in the element at the end of the list. Will return
        None in an empty list.

        Returns:
            The data in the node at last index of the list or None if there is no such node
        """
        def get_back_recursive(current, previous):
            """
            cycle down the link list to find the val, and remove it
            """
            if current is not None and current.next != self.tail:
                return get_back_recursive(current.next, current)
            if current is not None:
                return current.data

        return get_back_recursive(current=self.head, previous=self.head)  # initiate the recursion

    def remove_front(self):
        """
        Removes the first element of the list. Will not remove the tail.
        """
        top = self.head
        self.head = self.head.next
        return top.data

    def remove_back(self):
        """
        Removes the last element of the list. Will not remove the head.
        """
        def remove_back_recursive(current, previous):
            """
            cycle down the link list to find the val, and remove it
            """
            if current is not None and current.next != self.tail:
                return remove_back_recursive(current.next, current)
            if current is not None:
                previous.next = current.next

        return remove_back_recursive(current=self.head, previous=self.head)  # initiate the recursion

    def is_empty(self):
        """
        Checks if the list is empty

        Returns:
            True if the list has no data nodes, False otherwise
        """
        return self.head.next.data is None

    def contains(self, value):
        """
        Checks if any node contains value

        Args:
            value: The value to look for

        Returns:
            True if value is in the list, False otherwise
        """
        def contains_recursive(current):
            """
            cycles current to find the value
            """
            if current is not None:
                if current.data == value:
                    return True
                return contains_recursive(current.next)
            return False

        return contains_recursive(current=self.head)  # initiate the recursion

    def remove(self, value):
        """
        Removes the first instance of an element from the list

        Args:
            value: the value to remove
        """

        if self.head is None:  # If the list is empty
            return
        if self.head.data == value:  # If the node to remove is the head
            self.head = self.head.next
            return

        def remove_recursive(current, previous):
            """
            cycle down the link list to find the val, and remove it
            """
            if current is not None and current.data != value:
                return remove_recursive(current.next, current)
            if current is not None:
                previous.next = current.next

        return remove_recursive(current=self.head, previous=self.head)  # initiate the recursion

'''
**********************************************************************************
Part 2: Deque implemented with CircularlyDoublyLinked List
**********************************************************************************
'''

class DLNode:
    def __init__(self):
        self.next = None
        self.prev = None
        self.data = None

class CircularList:
    def __init__(self, start_list=None):
        """
        Initializes a linked list with a single sentinel node containing None data
        """
        self.sentinel = DLNode()
        self.sentinel.next = self.sentinel
        self.sentinel.prev = self.sentinel

        # populate list with initial set of nodes (if provided)
        if start_list is not None:
            for data in start_list:
                self.add_back(data)

    def __str__(self):
        """
        Returns a human readable string of the list content of the form
        [value1 <-> value2 <-> value3]

        An empty list should just print []

        Returns:
            The string of the human readable list representation
        """
        out = '['
        if self.sentinel.prev != self.sentinel:             
            cur = self.sentinel.next.next
            out = out + str(self.sentinel.next.data)
            while cur != self.sentinel:
                out = out + ' <-> ' + str(cur.data)
                cur = cur.next
        out = out + ']'
        return out

    def add_link_before(self, data, index):
        """
        Adds a new link containing data and inserts it before the link at index.
        If index is 0, it inserts at the beginning of the list.

        Args:
            data: The data the new node will contain
            index: The index of the node that will immediately follow the newly added node
        """
        new_link = DLNode()  # initialize a new link
        new_link.data = data  # set new_link data

        if self.sentinel.next is None:
            return
        if index == 0:  # check if the insert is at the head
            temp = self.sentinel.next
            self.sentinel.next = new_link
            self.sentinel.next.next = temp
            _temp = self.sentinel.prev
            self.sentinel.prev = new_link
            self.sentinel.prev.prev = _temp
            return

        def add_link_before_recursive_next(current, counter):
            """
            Cycles the link list until position is reached,
            or end of list is reached
            """
            if counter != index - 1:  # check if counter is the same value of the position(-1 for placement)
                if current.next.data is None:  # if next value is empty, insert node of val
                    current.next = new_link
                    return
                return add_link_before_recursive_next(current.next, counter + 1)  # move current, and counter down
            n_temp = current.next
            current.next = new_link
            current.next.next = n_temp

            def add_link_before_recursive_prev(_current, _counter, end):
                """
                Cycles the link list until position is reached,
                or end of list is reached
                """
                if _current.prev.data is not None and not end:  # run to the end of the list
                    return add_link_before_recursive_prev(_current.prev, _counter + 1, False)
                elif _current.prev.data is None:
                    return add_link_before_recursive_prev(self.sentinel.prev, _counter - index, True)
                if _counter != 0 and end:
                    return add_link_before_recursive_prev(_current.prev, _counter - 1, True)
                p_temp = _current.prev
                _current.prev = new_link
                _current.prev.prev = p_temp

            return add_link_before_recursive_prev(_current=self.sentinel.prev,
                                                  _counter=0, end=False)  # initiate the recursion

        return add_link_before_recursive_next(current=self.sentinel.next, counter=0)  # initiate the recursion

    def remove_link(self, index):
        """
        Removes the link at the location specified by index
        Args:
            Index: The index of the node that will be removed
        """
        if self.sentinel.next is None:  # If the list is empty
            return

        def remove_link_recursive_next(current, previous, counter):
            """
            cycle down the link list to find the val, and remove it
            """
            if current.data is not None and counter != index:
                return remove_link_recursive_next(current.next, current, counter + 1)
            previous.next = current.next

            def remove_link_recursive_prev(_current, _previous, _counter, end):
                """
                cycle down the link list to find the val, and remove it
                """
                if _current.data is not None and not end:  # run to the end of the list
                    return remove_link_recursive_prev(_current.prev, _current, _counter + 1, False)
                elif _current.data is None:
                    return remove_link_recursive_prev(self.sentinel.prev, self.sentinel, _counter - index - 1, True)
                if _counter != 0 and end:
                    return remove_link_recursive_prev(_current.prev, _current, _counter - 1, True)
                _previous.prev = _current.prev

            return remove_link_recursive_prev(_current=self.sentinel.prev,
                                              _previous=self.sentinel, _counter=0, end=False)  # initiate the recursion

        return remove_link_recursive_next(current=self.sentinel.next,
                                          previous=self.sentinel, counter=0)  # initiate the recursion

    def add_front(self, data):
        """
        Adds a new node at the beginning of the list that contains data

        Args:
            data: The data the new node will contain
        """
        new_link = DLNode()  # initialize a new link
        new_link.data = data  # set new_link data

        temp = self.sentinel.next
        self.sentinel.next = new_link
        self.sentinel.next.next = temp

        def add_front_recursive(current):
            """
            Uses recursion to cycle down current until next is None to add node
            """
            if current.prev.data is not None:  # if the coming value is not None
                return add_front_recursive(current.prev)
            _temp = current.prev
            current.prev = new_link
            current.prev.prev = _temp

        return add_front_recursive(current=self.sentinel)  # initiate the recursion

    def add_back(self, data):
        """
        Adds a new node at the end of the list that contains data

        Args:
            data: The data the new node will contain
        """
        new_link = DLNode()  # initialize a new link
        new_link.data = data  # set new_link data

        temp = self.sentinel.prev
        self.sentinel.prev = new_link
        self.sentinel.prev.prev = temp

        def add_back_recursive(current):
            """
            Uses recursion to cycle down current until next is None to add node
            """
            if current.next.data is not None:  # if the coming value is not None
                return add_back_recursive(current.next)
            _temp = current.next
            current.next = new_link
            current.next.next = _temp

        return add_back_recursive(current=self.sentinel)  # initiate the recursion

    def get_front(self):
        """
        Returns the data in the element at the front of the list. Will return
        None in an empty list.

        Returns:
            The data in the node at index 0 or None if there is no such node
        """
        return self.sentinel.next.data

    def get_back(self):
        """
        Returns the data in the element at the end of the list. Will return
        None in an empty list.

        Returns:
            The data in the node at last index of the list or None if there is no such node
        """
        def get_back_recursive(current):
            """
            cycle down the link list to find the val, and remove it
            """
            if current.data is not None and current.next.data is not None:
                return get_back_recursive(current.next)
            return current.data

        return get_back_recursive(current=self.sentinel.next)  # initiate the recursion

    def remove_front(self):
        """
        Removes the first element of the list. Will not remove the tail.
        """
        top = self.sentinel.next
        self.sentinel.next = self.sentinel.next.next

        def remove_front_recursive(current, previous):
            """
            cycle down the link list to find the val, and remove it
            """
            if current.data is not None and current.prev.data is not None:
                return remove_front_recursive(current.prev, current)
            previous.prev = current.prev

        return remove_front_recursive(current=self.sentinel.prev,
                                      previous=self.sentinel.prev) and top.data  # initiate the recursion

    def remove_back(self):
        """
        Removes the last element of the list. Will not remove the head.
        """
        bottom = self.sentinel.prev
        self.sentinel.prev = self.sentinel.prev.prev

        def remove_front_recursive(current, previous):
            """
            cycle down the link list to find the val, and remove it
            """
            if current.data is not None and current.next.data is not None:
                return remove_front_recursive(current.next, current)
            previous.next = current.next

        return remove_front_recursive(current=self.sentinel.next,
                                      previous=self.sentinel.next) and bottom.data  # initiate the recursion

    def is_empty(self):
        """
        Checks if the list is empty

        Returns:
            True if the list has no data nodes, False otherwise
        """
        return self.sentinel.next.data is None and self.sentinel.prev.data is None

    def contains(self, value):
        """
        Checks if any node contains value

        Args:
            value: The value to look for

        Returns:
            True if value is in the list, False otherwise
        """

        def contains_recursive(current):
            """
            cycles current to find the value
            """
            if current.data is not None:
                if current.data == value:
                    return True
                return contains_recursive(current.next)
            return False

        return contains_recursive(current=self.sentinel.next)  # initiate the recursion

    def remove(self, value):
        """
        Removes the first instance of an element from the list

        Args:
            value: the value to remove
        """
        if self.sentinel.next is None:  # If the list is empty
            return

        def remove_recursive_next(current, previous):
            """
            cycle down the link list to find the val, and remove it
            """
            if current.data is not None and current.data != value:
                return remove_recursive_next(current.next, current)
            if current.data == value:
                previous.next = current.next

                def remove_recursive_prev(_current, _previous):
                    """
                    cycle down the link list to find the val, and remove it
                    """
                    if _current.data is not None and _current.data != value:
                        return remove_recursive_prev(_current.prev, _current)
                    if _current.data == value:
                        _previous.prev = _current.prev

                return remove_recursive_prev(_current=self.sentinel.prev,
                                             _previous=self.sentinel)  # initiate the recursion

        return remove_recursive_next(current=self.sentinel.next, previous=self.sentinel)  # initiate the recursion

    def circularListReverse(self):
        """
        Reverses the order of the links. It must not create any additional new links to do so.
        (e.g. you cannot call DLNode()). If the list printed by following next was 0, 1, 2, 3,
        after the call it will be 3,2,1,0
        """
        def reverse_recursive_next(current, previous):
            """
            cycles down the link list, reversing the order
            """
            if current.data is None:  # check if current has value
                def reverse_recursive_prev(_current, _previous):
                    """
                    cycles down the link list, reversing the order
                    """
                    if _current.data is None:  # check if current has value
                        return _previous  # if current is None, set self.sentinel.prev to previous
                    _temp = _current.prev
                    _current.prev = _previous
                    _previous = _current
                    _current = _temp
                    return reverse_recursive_prev(_current, _previous)

                self.sentinel.prev = reverse_recursive_prev(_current=self.sentinel.prev,
                                                            _previous=self.sentinel)  # initiate the recursion
                return previous  # if current is None, set self.sentinel.next to previous
            temp = current.next
            current.next = previous
            previous = current
            current = temp
            return reverse_recursive_next(current, previous)

        self.sentinel.next = reverse_recursive_next(current=self.sentinel.next, previous=self.sentinel)  # initiate the recursion
