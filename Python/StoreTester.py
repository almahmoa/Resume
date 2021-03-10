# Author: Abraham Almahmoud
# Date: 01/11/2020
# Description: In addition to your file containing the code for the above classes,
#              you must also submit a file that contains unit tests for your Store.py file.
#              It must have at least five unit tests and use at least three different assert functions.

import unittest
from Store import Store, Product, Customer, InvalidCheckoutError


class TestProjects(unittest.TestCase):
    """Contains unit tests for the Store class"""

    def test1(self):  # Test if product's quantity decreases
        p = Product(1111, "Potato", "this is a potato", 4.99, 2)
        self.assertEqual(p.decrease_quantity(), 1)

    def test2(self):  # Test if product's title is correct
        p = Product(1111, "Potato", "this is a potato", 4.99, 2)
        self.assertNotEqual(p.get_title(), "Tomato")

    def test3(self):  # Test if customer is a premium member
        c = Customer("Kathy Premium", "0502", True)
        self.assertTrue(c.is_premium_member())

    def test4(self):  # Test if customer's ID is correct
        c = Customer("Kathy Premium", "0502", True)
        self.assertEqual(c.get_account_id(), "0502")

    def test5(self):  # Test if InvalidCheckoutError is raised correctly
        c = Customer("Kathy Premium", "0502", True)
        with self.assertRaises(InvalidCheckoutError):
            Store.check_out_member(c.get_account_id())


if __name__ == '__main__':
    unittest.main()
