var coins = [
'r1',
'r2',
'r3',
'r4',
'r5',
'r6',
'r7',
'r8',
'r9',
'r10',
'r11',
'r12',
'r13',
'r14',
    'r15'];

var pixels = [
    -3220,
-3300,
-3380,
-3460,
-3540,
-3620,
-3700,
-3780,
-3860,
-3940,
-4020,
-4100,
-4180,
-4260,
-4340,

];
var bo = -3220;
for (var i = 0; i < 15; i++) {
    console.log(' ' + bo);
    bo -= 80;
}

var inputFelt = document.getElementsByName('name');
var but1 = document.getElementById('but1');
var but2 = document.getElementById('but2');
var but3 = document.getElementById('but3');
var but4 = document.getElementById('but4');
var but5 = document.getElementById('but5');
var but6 = document.getElementById('but6');
var but7 = document.getElementById('but7');
var but8 = document.getElementById('but8');
var but9 = document.getElementById('but9');

//for (var i = 1; i < 10; i++) {
//    console.log('var but'+i +' = document.getElementById(but'+i+');'  ); 
//}

function amount(amount1) {
    if (amount1 == 'clear') {
        inputFelt[0].value = "";
    } else {
        cal(amount1);
    }

    
}

function cal(amount1) {
    if (inputFelt[0].value == "") {
        inputFelt[0].value = 0;
    }

    var val = parseFloat(inputFelt[0].value);
    if (amount1 == 0.5) {
        val *= amount1;
    }
    else if (amount1 == -1) {
        val += 10000000;
    }
    else if (amount1 == 2) {
        val *= amount1;
    }
    else {
        val += amount1;
    }

    inputFelt[0].value = val.toFixed(2);
}
