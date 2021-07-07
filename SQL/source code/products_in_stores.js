module.exports = function () {
    var express = require('express');
    var router = express.Router();

    function getStores(res, mysql, context, complete) {
        mysql.pool.query("SELECT store_id FROM stores", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getStores = results;
            complete();
        });
    }

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

    function getProductsinStores(res, mysql, context, complete) {
        mysql.pool.query("SELECT products_in_stores_id as id, stores.store_id AS store_p, products.product_id AS product_p FROM products_in_stores INNER JOIN stores ON store_p = stores.store_id INNER JOIN products ON product_p = products.product_id", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProductsinStores = results;
            complete();
        });
    }

    function getProductsinStore(res, mysql, context, id, complete) {
        console.log(id)
        var sql = "SELECT products_in_stores_id as id, stores.store_id AS store_p, products.product_id AS product_p FROM products_in_stores INNER JOIN stores ON store_p = stores.store_id INNER JOIN products ON product_p = products.product_id WHERE products_in_stores.products_in_stores_id = ?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProductsinStore = results[0];
            complete();
        });
    }

    function getProductsByStores(req, res, mysql, context, complete) {
        var query = "SELECT products_in_stores_id as id, stores.store_id AS store_p, products.product_id AS product_p FROM products_in_stores INNER JOIN stores ON store_p = stores.store_id INNER JOIN products ON product_p = products.product_id WHERE products_in_stores.store_p = ?";
        console.log(req.params)
        var inserts = [req.params.store_p]
        mysql.pool.query(query, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProductsinStores = results;
            complete();
        });
    }

    router.get('/', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts_in_store.js", "filterproductsinstores.js"];
        var mysql = req.app.get('mysql');
        getProductsinStores(res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        getProducts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 3) {
                res.render('products_in_stores', context);
            }

        }
    });

    router.get('/filter/:store_p', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts_in_store.js", "filterproductsinstores.js"];
        var mysql = req.app.get('mysql');
        getProductsByStores(req, res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        getProducts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 3) {
                res.render('products_in_stores', context);
            }
        }
    });

    router.get('/:id', function (req, res) {
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updateproducts_in_store.js"];
        var mysql = req.app.get('mysql');
        getProductsinStore(res, mysql, context, req.params.id, complete);
        getStores(res, mysql, context, complete);
        getProducts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 3) {
                res.render('update-products_in_stores', context);
            }
        }
    });

    router.post('/', function (req, res) {
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO products_in_stores (store_p, product_p) VALUES (?,?)";
        var inserts = [req.body.store_p, req.body.product_p];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            } else {
                res.redirect('/products_in_stores');
            }
        });
    });

    router.put('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE products_in_stores SET store_p=?, product_p=? WHERE products_in_stores_id = ?";
        var inserts = [req.body.store_p, req.body.product_p, req.params.id];
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
        var sql = "DELETE FROM products_in_stores WHERE products_in_stores_id = ?";
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