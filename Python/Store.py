# Author: Abraham Almahmoud
# Date: 01/11/2020
# Description: You will be writing a (rather primitive) online store simulator. It will have three classes: Product,
#              Customer and Store. All data members of each class should be marked as private and the classes should
#              have any get or set methods that will be needed to access them.


class InvalidCheckoutError(Exception):
    pass


class Product:
    """instantiate a product as an object"""
    def __init__(self, id_code, title, description, price, quantity_available):
        self.id_code = id_code
        self.title = title
        self.description = description
        self.price = price
        self.quantity_available = quantity_available

    def get_id_code(self):
        return self.id_code

    def get_title(self):
        return self.title

    def get_description(self):
        return self.description

    def get_price(self):
        return self.price

    def get_quantity_available(self):
        return self.quantity_available

    def decrease_quantity(self):
        """decreases the quantity available by one"""
        return self.quantity_available - 1


class Customer:
    """instantiate a customer as an object"""
    def __init__(self, name, account_id, premium_member):
        self.name = name
        self.account_id = account_id
        self.premium_member = premium_member
        self.cart = []  # customer's cart

    def get_name(self):
        return self.name

    def get_account_id(self):
        return self.account_id

    def is_premium_member(self):
        """returns whether the customer is a premium member"""
        return self.premium_member

    def add_product_to_cart(self, p_id):
        """adds the product ID code to the Customer's cart"""
        return self.cart.append(p_id)

    def get_cart(self):
        return self.cart

    def empty_cart(self):
        """empties the Customer's cart"""
        return self.cart.clear()


class Store:
    """Uses the objects 'product' and 'customer' to create a store"""
    def __init__(self):
        """"""
        self.inventory = []
        self.member = []

    def add_product(self, p):
        """adds a product to the inventory"""
        self.inventory.append(p)

    def add_member(self, c):
        """adds a customer to the members"""
        self.member.append(c)

    def get_product_from_id(self, p_id):
        """returns the Product with the matching ID. If no matching ID is found, it returns the special value None"""
        for product in self.inventory:
            if product.get_id_code() == p_id:
                return product
        return None

    def get_member_from_id(self, m_id):
        """returns the Customer with the matching ID. If no matching ID is found, it returns the special value None"""
        for customer in self.member:
            if customer.get_account_id() == m_id:
                return customer
        return None

    def product_search(self, p_search):
        """return a sorted list of ID codes for every product whose title or description contains the search string."""
        id_list = []
        for products in self.inventory:
            if p_search.lower() in products.get_title().lower() + products.get_description().lower():
                id_list.append(products.get_id_code())
        id_list.sort()
        return id_list

    def add_product_to_member_cart(self, p_id, m_id):
        p = self.get_product_from_id(p_id)
        c = self.get_member_from_id(m_id)

        if not self.get_product_from_id(p_id):  # check if product is not in inventory
            return "product ID not found"

        if p.get_quantity_available() <= 0:  # checks if inventory is in stock
            return "product out of stock"

        if not self.get_member_from_id(m_id):  # checks if member's ID is in the system
            return "member ID not found"

        c.add_product_to_cart(p.get_id_code())  # add product to member's cart
        return "product added to cart"

    def check_out_member(self, m_id):
        """proceeds with check out linking the member's id with cart"""
        check_out = 0  # stores the total cost of order

        if not self.get_member_from_id(m_id):  # if member's ID is not in the system, raise error.
            raise InvalidCheckoutError

        c = self.get_member_from_id(m_id)

        for i in c.get_cart():
            p = self.get_product_from_id(i)
            if p.get_quantity_available() > 0:  # check if product is still in stock to check out
                p.decrease_quantity()  # decrease the quantity of the product by one
                check_out += p.get_price()  # add products cost to check out
            else:
                print("out of stock")

        if not c.is_premium_member():
            check_out += round(check_out * 0.07, 2)  # add 7% to the total cost for shipping
        c.empty_cart()
        return check_out


def main():
    s = Store()
    p1 = Product(1111, "Potato", "this is a potato", 4.99, 1)
    p2 = Product(5123, "Tomato", "this is a tomato", 2.99, 0)
    p3 = Product(1234, "Red Potato", "this is a red potato", 1.69, 10)
    p4 = Product(2333, "Hot House", "this hot tomato", 10.55, 1)

    c1 = Customer("Dave Man", "0001", False)
    c2 = Customer("Kathy Feman", "0502", True)
    c3 = Customer("Mark Duck", "0032", False)

    s.add_product(p1)
    s.add_product(p2)
    s.add_product(p3)
    s.add_product(p4)
    s.add_member(c1)
    s.add_member(c2)
    try:
        s.product_search("potato")
        s.add_product_to_member_cart(1111, "0502")
        s.add_product_to_member_cart(1111, "0001")
        s.add_product_to_member_cart(5123, "0001")
        s.add_product_to_member_cart(1234, "0502")
        s.add_product_to_member_cart(2333, "0502")
        s.check_out_member("0502")
        s.add_product_to_member_cart(5123, "0001")
        s.add_product_to_member_cart(1111, "0001")
        print(s.check_out_member("0001"))
    except InvalidCheckoutError:
        print("member ID not found")


if __name__ == '__main__':
    main()
