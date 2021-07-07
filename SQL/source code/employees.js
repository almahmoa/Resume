module.exports = function () {
    var express = require('express');
    var router = express.Router();

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

    function getEmployees(res, mysql, context, complete) {
        mysql.pool.query("SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id", function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getEmployees = results;
            complete();
        });
    }

    function getEmployee(res, mysql, context, id, complete) {
        console.log(id)
        var sql = "SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id WHERE employee_id = ?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.employee = results[0];
            complete();
        });
    }

          function getEmployeesbyStores(req, res, mysql, context, complete){
      var query = "SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id WHERE employees.store_e = ?";
      console.log(req.params)
      var inserts = [req.params.store_e]
      mysql.pool.query(query, inserts, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getEmployees = results;
            complete();
        });
    }

      function ascEmployeesbyLastName(req, res, mysql, context, complete){
      var query = "SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id ORDER BY employees.last_name ASC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getEmployees = results;
            complete();
        });
    }

          function descEmployeesbyLastName(req, res, mysql, context, complete){
      var query = "SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id ORDER BY employees.last_name DESC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getEmployees = results;
            complete();
        });
    }

    function getEmployeesWithLastNameLike(req, res, mysql, context, complete) {
       var query = "SELECT employee_id as id, stores.store_id AS store_e, last_name, first_name, birth_date, email, phone, wage, gender FROM employees INNER JOIN stores ON store_e = stores.store_id WHERE employees.last_name LIKE " + mysql.pool.escape(req.params.s + '%');
      console.log(query)
      mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getEmployees = results;
            complete();
        });
    }

    router.get('/', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteemployees.js","filteremployees.js","searchemployees.js"];
        var mysql = req.app.get('mysql');
        getEmployees(res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 2) {
                res.render('employees', context);
            }

        }
    });

                router.get('/asc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteemployees.js","filteremployees.js","searchemployees.js"];
        var mysql = req.app.get('mysql');
        ascEmployeesbyLastName(req, res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 2){
                res.render('employees', context);
            }
        }
    });

                router.get('/desc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteemployees.js","filteremployees.js","searchemployees.js"];
        var mysql = req.app.get('mysql');
        descEmployeesbyLastName(req, res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 2){
                res.render('employees', context);
            }
        }
    });
    
    router.get('/filter/:store_e', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteemployees.js","filteremployees.js","searchemployees.js"];
        var mysql = req.app.get('mysql');
        getEmployeesbyStores(req, res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 2){
                res.render('employees', context);
            }
        }
    });

    router.get('/search/:s', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deleteemployees.js","filteremployees.js","searchemployees.js"];
        var mysql = req.app.get('mysql');
        getEmployeesWithLastNameLike(req, res, mysql, context, complete);
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 2){
                res.render('employees', context);
            }
        }
    });

    router.post('/', function (req, res) {
        // test
        // console.log("inserted")
        console.log(req.body.store_e)
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO employees (store_e, last_name, first_name, birth_date, email, phone, wage, gender) VALUES (?,?,?,?,?,?,?,?)";
        var inserts = [req.body.store_e, req.body.last_name, req.body.first_name, req.body.birth_date, req.body.email, req.body.phone, req.body.wage, req.body.gender];
        sql = mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            } else {
                res.redirect('/employees');
            }
        });
    });

    router.get('/:id', function (req, res) {
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updateemployee.js"];
        var mysql = req.app.get('mysql');
        getEmployee(res, mysql, context, req.params.id, complete);
        getStores(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 2) {
                res.render('update-employee', context);
            }

        }
    });

    router.put('/:id', function (req, res) {
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE employees SET store_e=?, last_name=?, first_name=?, birth_date=?, email=?, phone=?, wage=?, gender=? WHERE employee_id=?";
        var inserts = [req.body.store_e, req.body.last_name, req.body.first_name, req.body.birth_date, req.body.email, req.body.phone, req.body.wage, req.body.gender, req.params.id];
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
        var sql = "DELETE FROM employees WHERE employee_id = ?";
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