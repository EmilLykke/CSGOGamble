

var inputFelt = document.getElementsByName('name');
var coins = [];
var last_100 = {counter: 0, terrorist: 0, jackpot: 0}
var winRound = {update: false, amount: 0};

$(function () {
    runto(-1);
    CountDownTimer(new Date($("#onLoadRoundDate").data("date")));
    var chat = $.connection.bettingHub;
    chat.client.sendNext = function (time, number) {
        CountDownTimer(new Date(time))
    };
    chat.client.sendResult = function (result, counter, terrorist, jackpot) {
        last_100 = { counter: counter, terrorist: terrorist, jackpot: jackpot}
        $("#wheel-overlay-dark").fadeOut(500);
        var number = parseInt(result, 10)
        if (number == 0) {
            runto(14)
        } else {
            runto(number-1)
        }
    };
    chat.client.sendNewBet = function (username, amount, color) {
        addNewBet(color, amount, username);
    }

    chat.client.sendNewAmount = function (amount) {
        winRound.update = true;
        winRound.amount = amount;
    }
});

$.connection.hub.start().done(function () {}).fail(function (error) {alert("Failed to connect!");});

function bet(color) {
    var amount = $('input[name=name]').val()
    if (parseFloat(amount) > parseFloat($("#balance").text())) {
        return;
    }

    $.post('https://localhost:44344//Api/Bet', { amountString: amount, color: color }, function (data1) {
        if (!data1.eror) {
            $("#balance").text((data1.newAmount).toFixed(2))
        } else {
        }
    })
}

function CountDownTimer(dt) {
    if (dt == NaN) {
        return;
    }
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
    var repeats = backgroundWidth * Math.floor(Math.random() * (10 - 7 + 1) + 7)
    var numberOffset = Math.floor(Math.random() * (numberWidth - -numberWidth + 1) + -numberWidth)
    var gotoWidth = -repeats - (numberWidth * id) - numberWidth / 2 + $("#wheel").width() / 2 + numberOffset / 2
    if (id == -1) {
        
        $("#wheel").animate({ 'background-position-x': -(numberWidth * 14) - numberWidth / 2 + $("#wheel").width() / 2 + 'px' }, 1000, "swing")
        return;
    }
    disableButs();
    //$("#wheel").css('background-position-x', gotoWidth + "px")
    $("#wheel").animate({ 'background-position-x': gotoWidth + 'px' }, 10000, "swing", function () {
        setTimeout(function () {
            $("#wheel-overlay-dark").fadeIn(500);
            pushCoin(id);
            last100();
            displayButs();
            if (winRound.update) {
                $("#balance").text((winRound.amount).toFixed(2))
                winRound.update = false;
            }
            $("#wheel").animate({ 'background-position-x': -repeats - (numberWidth * 14) - numberWidth / 2 + $("#wheel").width() / 2 + 'px' }, 1000, "swing", function () {
                $("#wheel").css('background-position-x', -(numberWidth * 14) - numberWidth / 2 + $("#wheel").width() / 2 + 'px')
            })
        }, 2000)
    });
}

function displayButs(){
    document.getElementById('ct-bet').disabled = false;
    document.getElementById('jack-bet').disabled = false;
    document.getElementById('t-bet').disabled = false;
    var bets = document.getElementsByClassName("bets-style");
    for (var i = 0; i < bets.length; i++) {
        bets[i].style.opacity = 1;
    }
}

function disableButs() {
    document.getElementById('ct-bet').disabled = true;
    document.getElementById('jack-bet').disabled = true;
    document.getElementById('t-bet').disabled = true;
    var bets=document.getElementsByClassName("bets-style");
    for (var i = 0; i < bets.length; i++) {
        bets[i].style.opacity = 0.2;
    }
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
        val = parseFloat($("#balance").text());
    }
    else if (amount1 == 2) {
        val *= amount1;
    }
    else {
        val += amount1;
    }

    inputFelt[0].value = val.toFixed(2);
}

// 0 coin
// 1 counter 
var last10_1 = document.getElementById('last10-1');
var last10_2 = document.getElementById('last10-2');
var last10_3 = document.getElementById('last10-3');
var last10_4 = document.getElementById('last10-4');
var last10_5 = document.getElementById('last10-5');
var last10_6 = document.getElementById('last10-6');
var last10_7 = document.getElementById('last10-7');
var last10_8 = document.getElementById('last10-8');
var last10_9 = document.getElementById('last10-9');
var last10_10 = document.getElementById('last10-10');


for (var i = 0; i < 10; i++) {
    pushCoin(i);
}


function pushCoin(id) {
    if (coins.length > 9) {
        coins.shift();
        coins.push(id);
    } else {
        coins.push(id);
    }
    
    var num =0;
    for (var i = 0; i <= coins.length-1; i++) {
        num = i + 1;
        if (coins[i] == 0) {
            document.getElementById('last10-' + num).style.backgroundImage = "url('../Images/jackpot.svg')";
        } else if (coins[i] % 2 == 0) {
            
            document.getElementById('last10-' + num).style.backgroundImage = "url('../Images/counter.svg')";
            
        } else {
            
            document.getElementById('last10-' + num).style.backgroundImage = "url('../Images/terrorist.svg')";
        }
        
        
    }
    
}

for (var i = 0; i < 100; i++) {
    last100(i);
}

var counter = 0;
var terror = 0;
var jackpot = 0;

function last100() {
    document.getElementById('last100-jackpot').innerHTML = last_100.jackpot;
    document.getElementById('last100-counter').innerHTML = last_100.counter;
    document.getElementById('last100-terror').innerHTML = last_100.terrorist;
}





function addNewBet(color, amount, username) {
    var chosenCoin;
    if (color == "counter") {
        chosenCoin = "counter-entries";
    } else if (color == "jackpot") {
        chosenCoin = "jackpot-entries";
    } else if (color == "terrorist"){
        chosenCoin = "terror-entries";
    }

    var area = document.getElementById(chosenCoin);
    var firstDiv = document.createElement("div");
    firstDiv.className = "person-bet";
    var img = document.createElement("img");
    img.src = "Images/counter.svg";
    firstDiv.appendChild(img);
    var secondDiv = document.createElement("div");
    firstDiv.appendChild(secondDiv);

    var firstP = document.createElement("p");
    firstP.id = "personName";
    firstP.innerHTML = username;
    secondDiv.appendChild(firstP);
    var secondP = document.createElement("p");
    secondP.id = "amount";
    var val = document.getElementsByName("name");
    var val1 = val[0].value;
    
    secondP.innerHTML = amount;
    secondDiv.appendChild(secondP);

    area.appendChild(firstDiv);

}
