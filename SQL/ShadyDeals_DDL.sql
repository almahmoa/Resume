--
-- Table structure for table `customer`
--

DROP TABLE IF EXISTS `customers`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `customers` (
  `customer_id` int(11) NOT NULL AUTO_INCREMENT,
  `email` varchar(255) DEFAULT NULL,
  `birth_date` varchar(255) DEFAULT NULL,
  `membership_card` int NULL,
  PRIMARY KEY (`customer_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `customers`
--

LOCK TABLES `customers` WRITE;
/*!40000 ALTER TABLE `customers` DISABLE KEYS */;
INSERT INTO `customers` VALUES 
	(1,'yugi@mail.com', '1995-05-12', 1),
	(2,'kaiba@mail.com', '1993-08-04', 1),
	(3,'joey@mail.com', '1995-02-25', 0);
/*!40000 ALTER TABLE `customers` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products`
--

DROP TABLE IF EXISTS `products`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `products` (
  `product_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_name` varchar(255) NOT NULL,
  `product_company_id` varchar(255) NOT NULL,
  `cost` int(11) NOT NULL,
  `current_supply` int(11) NOT NULL,
  `max_supply` int(11) NOT NULL,
  `product_category` varchar(255) DEFAULT NULL,
  PRIMARY KEY (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `products` WRITE;
/*!40000 ALTER TABLE `products` DISABLE KEYS */;
INSERT INTO `products` (`product_id`, `product_name`, `product_company_id`, `cost`, `current_supply`, `max_supply`, `product_category`) VALUES
	('1', 'Sword & Shield: Chilling Reign Booster Box', 'Nintendo', '199.99', '2', '5000', 'TCG'),
	('2', 'Toon Chaos Booster Box', 'Konami', '299.99', '3000', '3000', 'TCG'),
	('3', 'Commander Legends Booster Box', 'Wizards of the Coast LLC', '119.99', '1643', '4000', 'TCG');
/*!40000 ALTER TABLE `products` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `stores`
--

DROP TABLE IF EXISTS `stores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `stores` (
  `store_id` int(11) NOT NULL AUTO_INCREMENT,
  `cost_of_rent` float(11) NOT NULL,
  `street` varchar(255) NOT NULL,
  `city` varchar(255) NOT NULL,
  `state` varchar(255) NOT NULL,
  `zip` int(11) NOT NULL,
  PRIMARY KEY (`store_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `stores` WRITE;
/*!40000 ALTER TABLE `stores` DISABLE KEYS */;
INSERT INTO `stores` (`store_id`, `cost_of_rent`, `street`, `city`, `state`, `zip`) VALUES
	(1, '5532', '72840 CA-111', 'Palm Desert', 'CA', '92261'),
	(2, '3250', '6816 Charlotte Pike STE 101', 'Nashville', 'TN', '37208'),
	(3, '4261', '930 Valkenburgh St Suite 206', 'Honolulu', 'HI', '96818');
/*!40000 ALTER TABLE `stores` ENABLE KEYS */;
UNLOCK TABLES;


--
-- Table structure for table `employees`
--

DROP TABLE IF EXISTS `employees`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `employees` (
  `employee_id` int(11) NOT NULL AUTO_INCREMENT,
  `store_e` int(11) DEFAULT NULL,
  `last_name` varchar(255) NOT NULL,
  `first_name` varchar(255) NOT NULL,
  `birth_date` varchar(255) NOT NULL,
  `email` varchar(255) NOT NULL,
  `phone` int(11) NOT NULL,
  `wage` int(11) NOT NULL,
  `gender` varchar(255) NOT NULL,
  PRIMARY KEY (`employee_id`),
  KEY `store_e` (`store_e`),
  CONSTRAINT `e_store_fk` FOREIGN KEY (`store_e`) REFERENCES `stores` (`store_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

ALTER TABLE employees
DROP CONSTRAINT e_store_fk; 

LOCK TABLES `employees` WRITE;
/*!40000 ALTER TABLE `employees` DISABLE KEYS */;
INSERT INTO `employees` (`employee_id`, store_e, `last_name`, `first_name`, `birth_date`, `email`, `phone`, `wage`, `gender`) VALUES 
	(1, 1,'Muto', 'Solomon', '1949-10-04', 'ApdnargOtum@mail.com', 7519237254, 5, 'male'),
	(2, 2, 'Devlin', 'Duke', '2005-02-28', 'dicemaster@mail.com', 3515213252, 30000, 'male'),
	(3, 3, 'Pegasus', 'Maxamillian', '1997-10-08', 'CeciliaForEva@mail.com', 1568791354, 10, 'male');
/*!40000 ALTER TABLE `employees` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products_in_carts`
--

DROP TABLE IF EXISTS `products_in_carts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `products_in_carts` (
  `cart_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_cart_id` int(11) NOT NULL,
  `product_c` int(11) NOT NULL,
    PRIMARY KEY (`cart_id`),
  KEY `product_cart_id` (`product_cart_id`),
  KEY `product_c` (`product_c`),
CONSTRAINT `pc_product_fk` FOREIGN KEY (`product_c`) REFERENCES `products` (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

ALTER TABLE products_in_carts
DROP CONSTRAINT pc_product_fk; 

LOCK TABLES `products_in_carts` WRITE;
/*!40000 ALTER TABLE `products_in_carts` DISABLE KEYS */;
INSERT INTO `products_in_carts` (`cart_id`, `product_cart_id`, `product_c`) VALUES
	(1, 1, 1),
	(2, 1, 2),
	(3, 1, 3),
	(4, 2, 1),
	(5, 2, 3),
	(6, 3, 2);
/*!40000 ALTER TABLE `products_in_carts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `products_in_carts`
--

DROP TABLE IF EXISTS `checkout_carts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `checkout_carts` (
  `checkout_id` int(11) NOT NULL AUTO_INCREMENT,
  `product_cart` int(11) NOT NULL,
  PRIMARY KEY (`checkout_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

LOCK TABLES `checkout_carts` WRITE;
/*!40000 ALTER TABLE `checkout_carts` DISABLE KEYS */;
INSERT INTO `checkout_carts` (`checkout_id`, `product_cart`) VALUES
	(1, 1),
	(2, 2),
	(3, 3);
/*!40000 ALTER TABLE `checkout_carts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `transactions`
--

DROP TABLE IF EXISTS `transactions`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `transactions` (
  `transaction_id` int(11) NOT NULL AUTO_INCREMENT,
  `customer_t` int(11) NOT NULL,
  `checkout_t` int(11) NOT NULL,
  `store_t` int(11) NOT NULL,
  `date` varchar(255) NOT NULL,
   PRIMARY KEY (`transaction_id`),
	KEY `customer_t` (`customer_t`),
	KEY `checkout_t` (`checkout_t`),
	KEY `store_t` (`store_t`),
	CONSTRAINT `t_customer_fk` FOREIGN KEY (`customer_t`) REFERENCES `customers` (`customer_id`),
	CONSTRAINT `t_checkout_fk` FOREIGN KEY (`checkout_t`) REFERENCES `checkout_carts` (`checkout_id`),
	CONSTRAINT `t_store_fk` FOREIGN KEY (`store_t`) REFERENCES `stores` (`store_id`)
) ENGINE=InnoDB AUTO_INCREMENT=3 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

ALTER TABLE transactions
DROP CONSTRAINT t_customer_fk,
DROP CONSTRAINT t_checkout_fk,
DROP CONSTRAINT t_store_fk; 

LOCK TABLES `transactions` WRITE;
/*!40000 ALTER TABLE `transactions` DISABLE KEYS */;
INSERT INTO `transactions` (`transaction_id`, `customer_t`, `checkout_t`, `store_t`, `date`) VALUES
	(1, 1, 1, 1, '1990-10-04'),
	(2, 2, 2, 1, '2000-11-05'),
	(3, 3, 3, 3, '2020-10-06');
/*!40000 ALTER TABLE `transactions` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `product_store`
--

DROP TABLE IF EXISTS `products_in_stores`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `products_in_stores` (
`products_in_stores_id` int(11) NOT NULL AUTO_INCREMENT,
  `store_p` int(11) DEFAULT NULL,
  `product_p` int(11) DEFAULT NULL,
     PRIMARY KEY (`products_in_stores_id`),
	KEY `store_p` (`store_p`),
	KEY `product_p` (`product_p`),
	CONSTRAINT `ps_store_fk` FOREIGN KEY (`store_p`) REFERENCES `stores` (`store_id`),
	CONSTRAINT `ps_product_fk` FOREIGN KEY (`product_p`) REFERENCES `products` (`product_id`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

ALTER TABLE products_in_stores
DROP CONSTRAINT ps_store_fk,
DROP CONSTRAINT ps_product_fk; 

LOCK TABLES `products_in_stores` WRITE;
/*!40000 ALTER TABLE `products_in_stores` DISABLE KEYS */;
INSERT INTO `products_in_stores` (`products_in_stores_id`, `store_p`, `product_p`) VALUES
	(1, 1, 1),
	(2, 1, 3),
	(3, 1, 1),
	(4, 2, 2),
	(5, 2, 3),
	(6, 3, 2);
/*!40000 ALTER TABLE `products_in_stores` ENABLE KEYS */;
UNLOCK TABLES;
