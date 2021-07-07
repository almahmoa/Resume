-- These are some Database Manipulation queries for a partially implemented Project Website 
-- using the bsg database.
-- Your submission should contain ALL the queries required to implement ALL the
-- functionalities listed in the Project Specs.

-- SELECT query to display all columns for table display in website
SELECT * FROM customers;
SELECT * FROM products;
SELECT * FROM stores;
SELECT * FROM employees;
SELECT * FROM transactions;
SELECT * FROM products_in_stores;
SELECT * FROM products_in_carts;
SELECT * FROM checkout_carts;

-- SELECT query to display all columns for table
SELECT customer_id as id, email, birth_date, membership_card FROM customers;
SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products;
SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores;
SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id;
SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id;
SELECT products_in_stores_id as id, stores.store_id AS store_p, products.product_id AS product_p FROM products_in_stores INNER JOIN stores ON store_p = stores.store_id INNER JOIN products ON product_p = products.product_id;
SELECT cart_id as id, product_cart_id, products.product_id AS product_c FROM products_in_carts INNER JOIN products ON product_c = products.product_id;
SELECT checkout_id as id, product_cart FROM checkout_carts;

-- SELECT query to display column with ID
SELECT customer_id as id, email, birth_date, membership_card FROM customers WHERE customer_id=?;
SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products WHERE product_id = ?;
SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores WHERE store_id=?;
SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id WHERE employee_id = ?;
SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id WHERE transaction_id = ?;
SELECT products_in_stores_id as id, stores.store_id AS store_p, products.product_id AS product_p FROM products_in_stores INNER JOIN stores ON store_p = stores.store_id INNER JOIN products ON product_p = products.product_id WHERE products_in_stores.products_in_stores_id = ?;
SELECT cart_id as id, product_cart_id, products.product_id AS product_c FROM products_in_carts INNER JOIN products ON product_c = products.product_id WHERE cart_id=?;
SELECT checkout_id as id, product_cart FROM checkout_carts WHERE checkout_id=?;

-- INSERT query into table
INSERT INTO customers (email, birth_date, membership_card) VALUES (?,?,?);
INSERT INTO products (product_name, product_company_id, cost, current_supply, max_supply, product_category) VALUES (?,?,?,?,?,?);
INSERT INTO stores (cost_of_rent, street, city, state, zip) VALUES (?,?,?,?,?);
INSERT INTO employees (store_e, last_name, first_name, birth_date, email, phone, wage, gender) VALUES (?,?,?,?,?,?,?,?);
INSERT INTO transactions (customer_t, checkout_t, store_t, date) VALUES (?,?,?,?);
INSERT INTO products_in_stores (store_p, product_p) VALUES (?,?);
INSERT INTO products_in_carts (product_cart_id, product_c) VALUES (?,?);
INSERT INTO checkout_carts (product_cart) VALUES (?);

-- UPDATE query into ID
UPDATE customers SET email=?, birth_date=?, membership_card=? WHERE customer_id=?;
UPDATE products SET product_name=?, product_company_id=?, cost=?, current_supply=?, max_supply=?, product_category=? WHERE product_id=?;
UPDATE stores SET cost_of_rent=?, street=?, city=?, state=?, zip=? WHERE store_id=?;
UPDATE employees SET store_e=?, last_name=?, first_name=?, birth_date=?, email=?, phone=?, wage=?, gender=? WHERE employee_id=?;
UPDATE transactions SET customer_t=?, checkout_t=?, store_t=?, date=? WHERE transaction_id=?;
UPDATE products_in_stores SET store_p=?, product_p=? WHERE products_in_stores_id = ?;
UPDATE products_in_carts SET product_cart_id=?, product_c=? WHERE cart_id = ?;
UPDATE checkout_carts SET product_cart=? WHERE checkout_id=?;

-- DELETE query into ID
DELETE FROM customers WHERE customer_id = ?;
DELETE FROM products WHERE product_id = ?;
DELETE FROM stores WHERE store_id = ?;
DELETE FROM employees WHERE employee_id = ?;
DELETE FROM transactions WHERE transaction_id = ?;
DELETE FROM products_in_stores WHERE products_in_stores_id = ?;
DELETE FROM products_in_carts WHERE cart_id = ?;
DELETE FROM checkout_carts WHERE checkout_id = ?;

-- FILTER query based on ?
SELECT customer_id as id, email, birth_date, membership_card FROM customers WHERE customers.membership_card = ?;
SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products WHERE products.product_company_id = ?;
SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores WHERE stores.state = ?;
SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id WHERE employees.store_e = ?;
SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id WHERE transactions.store_t = ?;
SELECT products_in_stores_id as id, stores.store_id AS store_p, products.product_id AS product_p FROM products_in_stores INNER JOIN stores ON store_p = stores.store_id INNER JOIN products ON product_p = products.product_id WHERE products_in_stores.store_p = ?;
SELECT cart_id as id, product_cart_id, products.product_id AS product_c FROM products_in_carts INNER JOIN products ON product_c = products.product_id WHERE products_in_carts.product_cart_id = ?;
