

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

function runto(id) {
    var backgroundHeight = $("#wheel").height();
    var backgroundWidth = backgroundHeight * 15
    var numberWidth = backgroundWidth / 15;
    var repeats;
    if (id == 0) {
        repeats = 0;
    } else {
        repeats = backgroundWidth * Math.floor(Math.random() * (10 - 5 + 1) + 5)
    }
    var numberOffset = Math.floor(Math.random() * (numberWidth - -numberWidth + 1) + -numberWidth)
    var gotoWidth = -repeats - (numberWidth * id) - numberWidth / 2 + $("#wheel").width() / 2 + numberOffset
    //$("#wheel").css('background-position-x', gotoWidth + "px")
    $("#wheel").animate({ 'background-position-x': gotoWidth + 'px' }, 3500, "swing", function () {
        console.log("Finished")
        setTimeout(function () {
            $("#wheel").animate({ 'background-position-x': -repeats -(numberWidth * 14) - numberWidth / 2 + $("#wheel").width() / 2 + 'px' }, 1000, "swing", function () {
                $("#wheel").css('background-position-x', -(numberWidth * 14) - numberWidth / 2 + $("#wheel").width() / 2 + 'px')
            })
        }, 2000)
    });
}

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
