$(document).ready(function () {
    $('#tblCustomers').DataTable();
});







$(function () {
    //Remove the dummy row if data present.
    if ($("#tblCustomers tr").length > 2) {
        $("#tblCustomers tr:eq(1)").remove();
    } else {
        var row = $("#tblCustomers tr:last-child");
        row.find(".Edit").hide();
        row.find(".Delete").hide();
        row.find("span").html('&nbsp;');
    }
});


function AppendRow(row, DoctorID, FirstName, LastName, BirthDate, Degree) {
    //Bind CustomerId.
    $(".DoctorId", row).find("span").html(DoctorID);

    //Bind FirstName.
    $(".FirstName", row).find("span").html(FirstName);
    $(".FirstName", row).find("input").val(FirstName);

    //Bind LastName.
    $(".LastName", row).find("span").html(LastName);
    $(".LastName", row).find("input").val(LastName);

    //Bind Birthdate
    $(".BirthDate", row).find("span").html(BirthDate);
    $(".BirthDate", row).find("input").val(BirthDate);

    //Bind Degree.
    $(".Degree", row).find("span").html(Degree);
    $(".Degree", row).find("input").val(Degree);

    row.find(".Edit").show();
    row.find(".Delete").show();
    $("#tblCustomers").append(row);
};




//Add event handler.
$("body").on("click", "#btnAdd", function () {
    var FirstName = $("#FirstName");
    var LastName = $("#LastName");
    var BirthDateId = $("#BirthDateId");
    var Degree = $("#Degree").val().split(" ");
    console.log(Degree);
    
    
    //var row = $("#tblCustomers tr:last-child");
    //AppendRow(row, DoctorID, FirstName, LastName, BirthDate, Degree);
    console.log(BirthDateId);
    $.ajax({
        type: "POST",
        url: "/Doctors/InsertDoctor",
        data: '{FirstName: "' + FirstName.val() + '", LastName: "' + LastName.val() + '", Birthdate: "' + BirthDateId.val() + '", Degree: "' + Degree[0] + '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (r) {
            var row = $("#tblCustomers tr:last-child");
            console.log("AAA");
            if ($("#tblCustomers tr:last-child span").eq(0).html() != "&nbsp;") {
                row = row.clone();
                console.log("Ekkhane aschi");
                console.log(r.FirstName);
                console.log(r.BirthDate);
            }
            AppendRow(row, r.DoctorID, r.FirstName, r.LastName, r.BirthDate, Degree[1]);
            FirstName.val("");
            LastName.val("");
            BirthDateId.val("");
            Degree.val("");
        }
    });
});


// Edit Event Handler
$("body").on("click", "#tblCustomers .Edit", function () {
    var row = $(this).closest("tr");
    $("td", row).each(function () {
        if ($(this).find("input").length > 0 || $(this).find("select").length > 0) {
            $(this).find("input").show();
            $(this).find("span").hide();
            $(this).find("select").show();
        }
    });

    row.find(".select").show();
    row.find(".Update").show();
    row.find(".Cancel").show();
    row.find(".Delete").hide();
    $(this).hide();
});



//Update event handler
$("body").on("click", "#tblCustomers .Update", function () {
    var row = $(this).closest("tr");
    var degree = row.find(".Degree").find("select").val().split()[0];
    var degree1 = degree.split(" ")[1];
    console.log(degree + "+" + degree1);
    $("td", row).each(function () {
        if ($(this).find("input").length > 0 || $(this).find("select").length > 0) {
            var span = $(this).find("span");
            var input = $(this).find("input");
            if (input.val() == undefined)
                span.html(degree1);
            else span.html(input.val());

            console.log($("this").find("select").val());
            span.show();
            input.hide();
            $(this).find("select").hide();
        }
    });
    row.find(".Edit").show();
    row.find(".Delete").show();
    row.find(".Cancel").hide();
    $(this).hide();
    //$("#Degree").val().split(" ");
    var doctor = {};
    doctor.DoctorID = row.find(".DoctorId").find("span").html();
    doctor.FirstName = row.find(".FirstName").find("span").html();
    doctor.LastName = row.find(".LastName").find("span").html();
    doctor.BirthDate = row.find(".BirthDate").find("span").html();
    
    doctor.Degree = degree.split(" ")[0];
    console.log(doctor.Degree);
    
    $.ajax({
        type: "POST",
        url: "/Doctors/UpdateDoctor",
        data: '{doctor:' + JSON.stringify(doctor) + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
});



//Cancel Handler
$("body").on("click", "#tblCustomers .Cancel", function () {
    var row = $(this).closest("tr");
    $("td", row).each(function () {
        if ($(this).find("input").length > 0 || $(this).find("select").length > 0) {
            var span = $(this).find("span");
            var input = $(this).find("input");
            input.val(span.html());
            span.show();
            input.hide();
            $(this).find("select").hide();
        }
    });
    row.find(".Edit").show();
    row.find(".Delete").show();
    row.find(".Update").hide();
    $(this).hide();
});





//Delete Event Handler
$("body").on("click", "#tblCustomers .Delete", function () {
    if (confirm("Do you want to delete this row?")) {
        var row = $(this).closest("tr");
        var doctorId = row.find("span").html();
        $.ajax({
            type: "POST",
            cache: false,
            url: "/Doctors/DeleteDoctor",
            data: '{DoctorID: ' + doctorId + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                
                if ($("#tblCustomers tr").length > 2) {
                    debugger;
                    row.remove();
                } else {
                    row.find(".Edit").hide();
                    row.find(".Delete").hide();
                    row.find("span").html('&nbsp;');
                }
            },
            error: function() {
                alert("Operation Failed");
            }
        });
    }
});


$("#selectDoctor").change(myfunc);


function myfunc(event) {
    var id = $("#selectDoctor").val();
    
    console.log(id);
    if(id!=0)
    $.ajax({
        url: "/Doctors/viewDetails/"+id,
        data: '{ id :'+ id +'}',
        cache: false,
        contentType: "application/json; charset=utf-8",
        type: "GET",
        dataType: "html",

        success: function (partialViewResult) {
            $("#schedule").html(partialViewResult); 
        },
        error: function () {
            alert("Kaj hoinai");
        }
        
    });
}

//$("#updateDays").click(myfunc2);

function myfunc2(event) {

    

    $.ajax({
        url: "/Doctors/viewDetails/",
        data: '{ days1 :' + $("#inp1").val() +
                ',days2 :' + $("#inp2").val() +
               
                ',days3 :' + $("#inp3").val() +
                ',days4 :' + $("#inp4").val() +
                ',days5 :' + $("#inp5").val() +
                ',days6 :' + $("#inp6").val() +
                ',days7 :' + $("#inp7").val() +
            '}',
        cache: false,
        contentType: "application/json; charset=utf-8",
        type: "POST",
        //dataType: "html",

        success: function () {
            //$("#schedule").html(partialViewResult);
            alert("asasas");
        },
        error: function () {
            alert("Kaj hoinai");
        }

    });

}









// Experimental
/*

$(".details").click(function () {
    var id = $("#selectDoctor").val();
    var row = $(this).closest("tr");
    var id = row.find("span").html();
    updateView(id);
    //console.log(id);
    location.replace("http://localhost:28103/Doctors/viewDetails");
    //setTimeout(function () {
        var ee = $("#selectDoctor").val();
        console.log("ee" + ee);
        
    //}, 10000);
    
   

});

function updateView(id) {
    console.log("Finall id");
    $.ajax({
        url: "/Doctors/viewDetails/" + id,
        data: '{ id :' + id + '}',
        cache: false,
        contentType: "application/json; charset=utf-8",
        type: "GET",
        dataType: "html",

        success: function (partialViewResult) {
            console.log("inner hello");
            $("#schedule").html(partialViewResult);
        },
        error: function () {
            alert("Kaj hoinai");
        }

    });

    $(document).ready(function () {
        
    })
} */