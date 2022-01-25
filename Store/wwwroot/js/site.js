function notification() {
    var connection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();
    //var x = document.getElementById("toast");
    connection.on("ReceiveMessage", function (approved, title) {
        //alert(x.innerHTML);
        document.getElementById("title").innerHTML = title;

        if (approved) {
            document.getElementById("desc").innerHTML = title + " Approved";
        } else {
            document.getElementById("desc").innerHTML = title + " Blocked";
        }
        $('.alert').alert();
        ////document.getElementById("notMsg").innerHTML = "hi";
        ////document.getElementById("desc").innerHTML ="mohd hi";
        //document.getElementById("toast").className = "show";
        //setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);

    });

    connection.start().then(function () {
    }).catch(function (err) {
        return console.error(err.toString());
    });
}