module.exports = function () {
    var express = require('express');
    var router = express.Router();

            function getCustomers(res, mysql, context, complete){
        mysql.pool.query("SELECT customer_id FROM customers", function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCustomers  = results;
            complete();
        });
    }

            function getCheckouts(res, mysql, context, complete){
        mysql.pool.query("SELECT checkout_id FROM checkout_carts", function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCheckouts  = results;
            complete();
        });
    }

            function getStores(res, mysql, context, complete){
        mysql.pool.query("SELECT store_id FROM stores", function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getStores  = results;
            complete();
        });
    }

    function getTransactions(res, mysql, context, complete) {
        mysql.pool.query("SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getTransactions = results;
            complete();
        });
    }

    
    function getTransaction(res, mysql, context, id, complete) {
        console.log(id)
        var sql = "SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id WHERE transaction_id = ?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.transaction = results[0];
            complete();
        });
    }

       function getTransactionsbyStores(req, res, mysql, context, complete){
      var query = "SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id WHERE transactions.store_t = ?";
      console.log(req.params)
      var inserts = [req.params.store_t]
      mysql.pool.query(query, inserts, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getTransactions = results;
            complete();
        });
    }

      function ascTransactionbyDate(req, res, mysql, context, complete){
      var query = "SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id  ORDER BY transactions.date ASC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getTransactions = results;
            complete();
        });
    }

          function descTransactionbyDate(req, res, mysql, context, complete){
      var query = "SELECT transaction_id as id, customers.customer_id AS customer_t, checkout_carts.checkout_id AS checkout_t, stores.store_id AS store_t, date FROM transactions INNER JOIN customers ON customer_t = customers.customer_id INNER JOIN checkout_carts ON checkout_t = checkout_carts.checkout_id INNER JOIN stores ON store_t = stores.store_id  ORDER BY transactions.date DESC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getTransactions = results;
            complete();
        });
    }

    router.get('/', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletetransactions.js","filtertransactions.js","searchtransactions.js"];
        var mysql = req.app.get('mysql');
        getTransactions(res, mysql, context, complete);
        getCustomers(res, mysql, context, complete);
        getCheckouts(res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 4) {
                res.render('transactions', context);
            }

        }
    });

     router.get('/asc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletetransactions.js","filtertransactions.js","searchtransactions.js"];
        var mysql = req.app.get('mysql');
        ascTransactionbyDate(req, res, mysql, context, complete);
        getCustomers(res, mysql, context, complete);
        getCheckouts(res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 4){
                res.render('transactions', context);
            }
        }
    });

                router.get('/desc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletetransactions.js","filtertransactions.js","searchtransactions.js"];
        var mysql = req.app.get('mysql');
        descTransactionbyDate(req, res, mysql, context, complete);
        getCustomers(res, mysql, context, complete);
        getCheckouts(res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 4){
                res.render('transactions', context);
            }
        }
    });
    
    router.get('/filter/:store_t', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletetransactions.js","filtertransactions.js","searchtransactions.js"];
        var mysql = req.app.get('mysql');
        getTransactionsbyStores(req, res, mysql, context, complete);
        getCustomers(res, mysql, context, complete);
        getCheckouts(res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 4){
                res.render('transactions', context);
            }
        }
    });

    router.post('/', function (req, res) {
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO transactions (customer_t, checkout_t, store_t, date) VALUES (?,?,?,?)";
        var inserts = [req.body.customer_t, req.body.checkout_t, req.body.store_t, req.body.date];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            } else {
                res.redirect('/transactions');
            }
        });
    });

    router.get('/:id', function (req, res) {
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updatetransaction.js"];
        var mysql = req.app.get('mysql');
        getTransaction(res, mysql, context, req.params.id, complete);
        getCustomers(res, mysql, context, complete);
        getCheckouts(res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 4) {
                res.render('update-transaction', context);
            }
        }
    });

    router.put('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE transactions SET customer_t=?, checkout_t=?, store_t=?, date=? WHERE transaction_id=?";
        var inserts = [req.body.customer_t, req.body.checkout_t, req.body.store_t, req.body.date, req.params.id];
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
        var sql = "DELETE FROM transactions WHERE transaction_id = ?";
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