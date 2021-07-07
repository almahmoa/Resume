module.exports = function () {
    var express = require('express');
    var router = express.Router();

    function getCheckoutCarts(res, mysql, context, complete){
        mysql.pool.query("SELECT checkout_id as id, product_cart FROM checkout_carts", function(error, results, fields){
            if(error){
                res.write(JSON.stringify(error));
                res.end();
            }
            context.getCheckoutCarts = results;
            complete();
        });
    }

    function getCheckoutCart(res, mysql, context, id, complete) {
        console.log(id)
        var sql = "SELECT checkout_id as id, product_cart FROM checkout_carts WHERE checkout_id=?";
        var inserts = [id];
        mysql.pool.query(sql, inserts, function (error, results, fields) {
            if (error) {
                res.write(JSON.stringify(error));
                res.end();
            }
            context.checkout = results[0];
            complete();
        });
    }

    router.get('/', function (req, res) {
        var callbackCount = 0;
        var context = {};
        context.jsscripts = ["deletecheckoutcarts.js"];
        var mysql = req.app.get('mysql');
        getCheckoutCarts(res, mysql, context, complete);
        function complete() {
            callbackCount++;
            if (callbackCount >= 1) {
                res.render('checkout_carts', context);
            }

        }
    });

    router.post('/', function(req, res){
        console.log(req.body)
        var mysql = req.app.get('mysql');
        var sql = "INSERT INTO checkout_carts (product_cart) VALUES (?)";
        var inserts = [req.body.product_cart];
        sql = mysql.pool.query(sql,inserts,function(error, results, fields){
            if(error){
                console.log(JSON.stringify(error))
                res.write(JSON.stringify(error));
                res.end();
            }else{
                res.redirect('/checkout_carts');
            }
        });
    });

    router.get('/:id', function(req, res){
        callbackCount = 0;
        var context = {};
        context.jsscripts = ["updatecheckoutcart.js"];
        var mysql = req.app.get('mysql');
        getCheckoutCart(res, mysql, context, req.params.id, complete);
        function complete(){
            callbackCount++;
            if(callbackCount >= 1){
                res.render('update-checkout_cart', context);
            }
        }
    });

    router.put('/:id', function(req, res){
        var mysql = req.app.get('mysql');
        console.log(req.body)
        console.log(req.params.id)
        var sql = "UPDATE checkout_carts SET product_cart=? WHERE checkout_id=?";
        var inserts = [req.body.product_cart, req.params.id];
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
        var sql = "DELETE FROM checkout_carts WHERE checkout_id = ?";
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