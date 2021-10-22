

var inputFelt = document.getElementsByName('name');
var coins = [];
var last_100 = {counter: 0, terrorist: 0, jackpot: 0}
var winRound = {update: false, amount: 0};

$(function () {
    runto(-1);
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

    $.connection.hub.start().done(function () {
        chat.server.getNextRound();
    }).fail(function (error) { alert("Failed to connect!"); });
});


function bet(color) {
    var amount = $('input[name=name]').val()
    if (parseFloat(amount) > parseFloat($("#balance").text())) {
        return;
    }

    $.post('/Api/Bet', { amountString: amount, color: color }, function (data1) {
        if (!data1.error) {
            console.log(data1)
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
            clearBets();
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


function pushCoin(id) {
    if ($('#last10-col').children().length > 9) {
        $('#last10-col').children()[0].remove()
    }
    if (id == 14) {
        $('#last10-col').append('<div class="last-margin-jackpot"></div>')
    } else if (id % 2 == 0) {
        $('#last10-col').append('<div class="last-margin-counter"></div>')
    } else {
        $('#last10-col').append('<div class="last-margin-terrorist"></div>')
    } 
}

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
    jackpottotal = 0;
    countertotal = 0;
    terroristtotal = 0;
    $("#jackpot-entries").children().each(function ( ) {
        jackpottotal += parseFloat($(this).find("#amount").html())
    })
    $("#terror-entries").children().each(function () {
        terroristtotal += parseFloat($(this).find("#amount").html())
    })
    $("#counter-entries").children().each(function () {
        countertotal += parseFloat($(this).find("#amount").html())
    })
    $("#counter-total-entries").text($("#counter-entries").children().length)
    $("#total-amount-counter").text(countertotal)

    $("#jackpot-total-entries").text($("#jackpot-entries").children().length)
    $("#total-amount-jackpot").text(jackpottotal)

    $("#total-amount-terror").text(terroristtotal)
    $("#terror-total-entries").text($("#terror-entries").children().length)
}

function clearBets() {
    $("#counter-total-entries").text(0)
    $("#total-amount-counter").text(0)

    $("#jackpot-total-entries").text(0)
    $("#total-amount-jackpot").text(0)

    $("#total-amount-terror").text(0)
    $("#terror-total-entries").text(0)

    $("#jackpot-entries").empty()

    $("#terror-entries").empty()

    $("#counter-entries").empty()
}
