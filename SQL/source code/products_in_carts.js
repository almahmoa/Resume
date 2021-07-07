module.exports = function () {
    var express = require('express');
    var router = express.Router();

        function getProducts(res, mysql, context, complete) {
        mysql.pool.query("SELECT product_id FROM products", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProducts = results;
            complete();
        });
    }

    function getProductsinCarts(res, mysql, context, complete) {
        mysql.pool.query("SELECT cart_id as id, product_cart_id, products.product_id AS product_c FROM products_in_carts INNER JOIN products ON product_c = products.product_id", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProductsinCarts = results;
            complete();
        });
    }

    function getProductsinCart(res, mysql, context, id, complete) {
        var sql = "SELECT cart_id as id, product_cart_id, products.product_id AS product_c FROM products_in_carts INNER JOIN products ON product_c = products.product_id WHERE cart_id=?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.productsinCart = results[0];
            complete();
        });
    }

        function getProductsByCarts(req, res, mysql, context, complete) {
        var query = "SELECT cart_id as id, product_cart_id, products.product_id AS product_c FROM products_in_carts INNER JOIN products ON product_c = products.product_id WHERE products_in_carts.product_cart_id = ?";
        console.log(req.params)
        var inserts = [req.params.product_cart_id]
        mysql.pool.query(query, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProductsinCarts = results;
            complete();
        });
    }

    router.get('/', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts_in_cart.js", "filterproductsincarts.js"];
        var mysql = req.app.get('mysql');
        getProductsinCarts(res, mysql, context, complete);
        getProducts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 2) {
                res.render('products_in_carts', context);
            }

        }
    });

        router.get('/filter/:product_cart_id', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts_in_cart.js", "filterproductsincarts.js"];
        var mysql = req.app.get('mysql');
        getProductsByCarts(req, res, mysql, context, complete);
        getProducts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 3) {
                res.render('products_in_carts', context);
            }
        }
    });

    router.get('/:id', function (req, res) {
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updateproducts_in_cart.js"];
        var mysql = req.app.get('mysql');
        getProductsinCart(res, mysql, context, req.params.id, complete);
        getProducts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 2) {
                res.render('update-products_in_cart', context);
            }

        }
    });

    router.post('/', function (req, res) {
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO products_in_carts (product_cart_id, product_c) VALUES (?,?)";
        var inserts = [req.body.product_cart_id, req.body.product_c];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            } else {
                res.redirect('/products_in_carts');
            }
        });
    });

    /* The URI that update data is sent to in order to update a person */

    router.put('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE products_in_carts SET product_cart_id=?, product_c=? WHERE cart_id = ?";
        var inserts = [req.body.product_cart_id, req.body.product_c, req.params.id];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(error)
                res.write(JSON.stringify(error));
                res.end();
            } else {
                res.status(200);
                res.end();
            }
        });
    });

    router.delete('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        var sql = "DELETE FROM products_in_carts WHERE cart_id = ?";
        var inserts = [req.params.id];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(error)
                res.write(JSON.stringify(error));
                res.status(400);
                res.end();
            } else {
                res.status(202).end();
            }
        })
    })


    return router;
}();

