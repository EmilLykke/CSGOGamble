

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


$(function () {
    runto(-1);
    CountDownTimer(new Date($("#onLoadRoundDate").data("date")));
    var chat = $.connection.bettingHub;
    chat.client.sendNext = function (time, number) {
        console.log(time)
        console.log(number)
        CountDownTimer(new Date(time))
    };
    chat.client.sendResult = function (result) {
        $("#wheel-overlay-dark").fadeOut(500);
        var number = parseInt(result, 10)
        if (number == 0) {
            runto(14)
        } else {
            runto(number-1)
        }
    };
});

$.connection.hub.start().done(function () {}).fail(function (error) {alert("Failed to connect!");});

function CountDownTimer(dt) {
    var end = new Date(dt);

    var timer;

    function showRemaining() {
        var now = new Date();
        var distance = end - now;
        $("#countdown-time").text((distance / 1000).toFixed(1));
        if (distance < 0) {
            clearInterval(timer);
            return;
        }
    }

    timer = setInterval(showRemaining, 100);
}

function runto(id) {

    var backgroundHeight = $("#wheel").height();
    var backgroundWidth = backgroundHeight * 15
    var numberWidth = backgroundWidth / 15;
    var repeats;
    if (id == 0) {
        repeats = 0;
    } else {
        repeats = backgroundWidth * Math.floor(Math.random() * (10 - 7 + 1) + 7)
    }
    var numberOffset = Math.floor(Math.random() * (numberWidth - -numberWidth + 1) + -numberWidth)
    var gotoWidth = -repeats - (numberWidth * id) - numberWidth / 2 + $("#wheel").width() / 2 + numberOffset / 2
    if (id == -1) {
        $("#wheel").animate({ 'background-position-x': -(numberWidth * 14) - numberWidth / 2 + $("#wheel").width() / 2 + 'px' }, 1000, "swing")
        return
    }
    //$("#wheel").css('background-position-x', gotoWidth + "px")
    $("#wheel").animate({ 'background-position-x': gotoWidth + 'px' }, 10000, "swing", function () {
        console.log("Finished")
        setTimeout(function () {
            $("#wheel-overlay-dark").fadeIn(500);
            $("#wheel").animate({ 'background-position-x': -repeats - (numberWidth * 14) - numberWidth / 2 + $("#wheel").width() / 2 + 'px' }, 1000, "swing", function () {
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



function pushCoin(id){
    var coins = [];

}



