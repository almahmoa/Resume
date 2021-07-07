module.exports = function () {
    var express = require('express');
    var router = express.Router();

    function getProducts(res, mysql, context, complete) {
        mysql.pool.query("SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProducts = results;
            complete();
        });
    }

    function getProduct(res, mysql, context, id, complete) {
        var sql = "SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products WHERE product_id = ?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.product = results[0];
            complete();
        });
    }

      function getProductsbyCompany(req, res, mysql, context, complete){
      var query = "SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products WHERE products.product_company_id = ?";
      console.log(req.params)
      var inserts = [req.params.product_company_id]
      mysql.pool.query(query, inserts, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProducts = results;
            complete();
        });
    }

      function ascProductsbySupply(req, res, mysql, context, complete){
      var query = "SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products ORDER BY products.current_supply ASC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProducts = results;
            complete();
        });
    }

          function descProductsbySupply(req, res, mysql, context, complete){
      var query = "SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products ORDER BY products.current_supply DESC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProducts = results;
            complete();
        });
    }


    function getProductsWithNameLike(req, res, mysql, context, complete) {
       var query = "SELECT product_id as id, product_name, product_company_id, cost, current_supply, max_supply, product_category FROM products WHERE products.product_name LIKE " + mysql.pool.escape(req.params.s + '%');
      console.log(query)

      mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getProducts = results;
            complete();
        });
    }

    /*Display all people. Requires web based javascript to delete users with AJAX*/

    router.get('/', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts.js","filterproducts.js","searchproducts.js"];
        var mysql = req.app.get('mysql');
        getProducts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 1) {
                res.render('products', context);
            }

        }
    });

              router.get('/asc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts.js","filterproducts.js","searchproducts.js"];
        var mysql = req.app.get('mysql');
        ascProductsbySupply(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('products', context);
            }
        }
    });

                router.get('/desc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts.js","filterproducts.js","searchproducts.js"];
        var mysql = req.app.get('mysql');
        descProductsbySupply(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('products', context);
            }
        }
    });
    
    router.get('/filter/:product_company_id', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts.js","filterproducts.js","searchproducts.js"];
        var mysql = req.app.get('mysql');
        getProductsbyCompany(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('products', context);
            }
        }
    });

    router.get('/search/:s', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteproducts.js","filterproducts.js","searchproducts.js"];
        var mysql = req.app.get('mysql');
        getProductsWithNameLike(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('products', context);
            }
        }
    });

    /* Display one person for the specific purpose of updating people */

    router.get('/:id', function (req, res) {
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updateproduct.js"];
        var mysql = req.app.get('mysql');
        getProduct(res, mysql, context, req.params.id, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 1) {
                res.render('update-product', context);
            }

        }
    });

    /* Adds a person, redirects to the people page after adding */

    router.post('/', function (req, res) {
        console.log(req.body.product_company_id)
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO products (product_name, product_company_id, cost, current_supply, max_supply, product_category) VALUES (?,?,?,?,?,?)";
        var inserts = [req.body.product_name, req.body.product_company_id, req.body.cost, req.body.current_supply, req.body.max_supply, req.body.product_category];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            } else {
                res.redirect('/products');
            }
        });
    });

    /* The URI that update data is sent to in order to update a person */

    router.put('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE products SET product_name=?, product_company_id=?, cost=?, current_supply=?, max_supply=?, product_category=? WHERE product_id=?";
        var inserts = [req.body.product_name, req.body.product_company_id, req.body.cost, req.body.current_supply, req.body.max_supply, req.body.product_category, req.params.id];
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

    /* Route to delete a person, simply returns a 202 upon success. Ajax will handle this. */

    router.delete('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        var sql = "DELETE FROM products WHERE product_id = ?";
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