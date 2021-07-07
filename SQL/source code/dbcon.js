var mysql = require('mysql');
var pool = mysql.createPool({
  connectionLimit : 10,
  host            : 'classmysql.engr.oregonstate.edu',
  user            : 'cs340_almahmoa',
  password        : '7181',
  database        : 'cs340_almahmoa'
});
module.exports.pool = pool;