module.exports = function(){
    var express = require('express');
    var router = express.Router();

    function getStores(res, mysql, context, complete){
        mysql.pool.query("SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores", function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getStores = results;
            complete();
        });
    }

    function getStore(res, mysql, context, id, complete){
        var sql = "SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores WHERE store_id=?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.store = results[0];
            complete();
        });
    }

      function getStoresbyState(req, res, mysql, context, complete){
      var query = "SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores WHERE stores.state = ?";
      console.log(req.params)
      var inserts = [req.params.state]
      mysql.pool.query(query, inserts, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getStores = results;
            complete();
        });
    }

      function ascStoresbyState(req, res, mysql, context, complete){
      var query = "SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores ORDER BY stores.state ASC, stores.street ASC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getStores = results;
            complete();
        });
    }

          function descStoresbyState(req, res, mysql, context, complete){
      var query = "SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores ORDER BY stores.state DESC, stores.street DESC";
         console.log(query)
       mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getStores = results;
            complete();
        });
    }


    function getCitiesWithNameLike(req, res, mysql, context, complete) {
       var query = "SELECT store_id as id, cost_of_rent, street, city, state, zip FROM stores WHERE stores.city LIKE " + mysql.pool.escape(req.params.s + '%');
      console.log(query)

      mysql.pool.query(query, function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getStores = results;
            complete();
        });
    }

    router.get('/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletestores.js", "filterstores.js","searchstores.js"];
        var mysql = req.app.get('mysql');
        getStores(res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('stores', context);
            }

        }
    });

            router.get('/asc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletestores.js","filterstores.js","searchstores.js"];
        var mysql = req.app.get('mysql');
        ascStoresbyState(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('stores', context);
            }
        }
    });

                router.get('/desc/', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletestores.js","filterstores.js","searchstores.js"];
        var mysql = req.app.get('mysql');
        descStoresbyState(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('stores', context);
            }
        }
    });
    
    router.get('/filter/:state', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletestores.js","filterstores.js","searchstores.js"];
        var mysql = req.app.get('mysql');
        getStoresbyState(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('stores', context);
            }
        }
    });

    router.get('/search/:s', function(req, res){
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletestores.js","filterstores.js","searchstores.js"];
        var mysql = req.app.get('mysql');
        getCitiesWithNameLike(req, res, mysql, context, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('stores', context);
            }
        }
    });

    /* Display one person for the specific purpose of updating people */

    router.get('/:id', function(req, res){
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updatestore.js"];
        var mysql = req.app.get('mysql');
        getStore(res, mysql, context, req.params.id, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('update-store', context);
            }
        }
    });

    /* Adds a person, redirects to the people page after adding */

    router.post('/', function(req, res){
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO stores (cost_of_rent, street, city, state, zip) VALUES (?,?,?,?,?)";
        var inserts = [req.body.cost_of_rent, req.body.street, req.body.city, req.body.state, req.body.zip];
        sql = mysql.pool.query(sql,inserts,function(error, results, fields){
            if(error){
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            }else{
                res.redirect('/stores');
            }
        });
    });

    /* The URI that update data is sent to in order to update a person */

    router.put('/:id', function(req, res){
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE stores SET cost_of_rent=?, street=?, city=?, state=?, zip=? WHERE store_id=?";
        var inserts = [req.body.cost_of_rent, req.body.street, req.body.city, req.body.state, req.body.zip, req.params.id];
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

    /* Route to delete a person, simply returns a 202 upon success. Ajax will handle this. */

    router.delete('/:id', function(req, res){
        var mysql = req.app.get('mysql');
        var sql = "DELETE FROM stores WHERE store_id = ?";
        var inserts = [req.params.id];
        sql = mysql.pool.query(sql, inserts, function(error, results, fields){
            if(error){
                console.log(error)
                res.write(JSON.stringify(error));
                res.status(400);
                res.end();
            }else{
                res.status(202).end();
            }
        })
    })

    return router;
}();
