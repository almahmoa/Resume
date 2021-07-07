module.exports = function () {
    var express = require('express');
    var router = express.Router();

    function getCustomers(res, mysql, context, complete) {
        mysql.pool.query("SELECT customer_id as id, email, birth_date, membership_card FROM customers", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCustomers = results;
            complete();
        });
    }

    function getCustomer(res, mysql, context, id, complete) {
        console.log(id)
        var sql = "SELECT customer_id as id, email, birth_date, membership_card FROM customers WHERE customer_id=?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.customer = results[0];
            complete();
        });
    }

    
      function getCustomersbyMembership(req, res, mysql, context, complete){
      var query = "SELECT customer_id as id, email, birth_date, membership_card FROM customers WHERE customers.membership_card = ?";
      console.log(req.params)
      var inserts = [req.params.membership_card]
      mysql.pool.query(query, inserts, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCustomers = results;
            complete();
        });
    }

      function ascCustomersbyBirth(req, res, mysql, context, complete){
      var query = "SELECT customer_id as id, email, birth_date, membership_card FROM customers ORDER BY customers.birth_date ASC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCustomers = results;
            complete();
        });
    }

          function descCustomersbyBirth(req, res, mysql, context, complete){
      var query = "SELECT customer_id as id, email, birth_date, membership_card FROM customers ORDER BY customers.birth_date DESC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCustomers = results;
            complete();
        });
    }


    function getEmailWithNameLike(req, res, mysql, context, complete) {
       var query = "SELECT customer_id as id, email, birth_date, membership_card FROM customers WHERE customers.email LIKE " + mysql.pool.escape(req.params.s + '%');
      console.log(query)

      mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCustomers = results;
            complete();
        });
    }
    
    router.get('/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletecustomers.js","filtercustomers.js","searchcustomers.js"];
        var mysql = req.app.get('mysql');
        getCustomers(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('customers', context);
            }

        }
    });
    
            router.get('/asc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletecustomers.js","filtercustomers.js","searchcustomers.js"];
        var mysql = req.app.get('mysql');
        ascCustomersbyBirth(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('customers', context);
            }
        }
    });

                router.get('/desc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletecustomers.js","filtercustomers.js","searchcustomers.js"];
        var mysql = req.app.get('mysql');
        descCustomersbyBirth(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('customers', context);
            }
        }
    });
    
    router.get('/filter/:membership_card', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletecustomers.js","filtercustomers.js","searchcustomers.js"];
        var mysql = req.app.get('mysql');
        getCustomersbyMembership(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('customers', context);
            }
        }
    });

    router.get('/search/:s', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts =  ["deletecustomers.js","filtercustomers.js","searchcustomers.js"];
        var mysql = req.app.get('mysql');
        getEmailWithNameLike(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('customers', context);
            }
        }
    });

 router.get('/:id', function(req, res){
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updatecustomer.js"];
        var mysql = req.app.get('mysql');
        getCustomer(res, mysql, context, req.params.id, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('update-customer', context);
            }
        }
    });

        router.post('/', function (req, res) {
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO customers (email, birth_date, membership_card) VALUES (?,?,?)";
        var inserts = [req.body.email, req.body.birth_date, req.body.membership_card];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            } else {
                res.redirect('/customers');
            }
        });
    });

    router.put('/:id', function(req, res){
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE customers SET email=?, birth_date=?, membership_card=? WHERE customer_id=?";
        var inserts = [req.body.email, req.body.birth_date, req.body.membership_card, req.params.id];
        sql = mysql.pool.query(sql,inserts,function(error, results, fields){
            if(error){
                console.log(error)
                res.write(JSON.stringify(error));
                res.end();
            }else{
                res.status(200);
                res.end();
            }
        });
    });

    router.delete('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        var sql = "DELETE FROM customers WHERE customer_id = ?";
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