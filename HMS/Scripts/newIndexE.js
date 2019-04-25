$(document).ready(function () {
    $('#tblExpertise').DataTable();
});



$(function () {
    //Remove the dummy row if data present.
    if ($("#tblExpertise tr").length > 2) {
        $("#tblExpertise tr:eq(1)").remove();
    } else {
        var row = $("#tblExpertise tr:last-child");
        row.find(".Edit").hide();
        row.find(".Delete").hide();
        row.find("span").html('&nbsp;');
    }
});


function AppendRow(row, ExpertiseID, Speciality) {
    //Bind ExpertiseId.
    $(".ExpertiseId", row).find("span").html(ExpertiseID);

    //Bind Speciality.
    $(".Speciality", row).find("span").html(Speciality);
    $(".Speciality", row).find("input").val(Speciality);


    row.find(".Edit").show();
    row.find(".Delete").show();
    $("#tblExpertise").append(row);
};




//Add event handler.
$("body").on("click", "#btnAdd", function () {
   // var ExpetiseId = $("#ExpertiseID");
    var Speciality = $("#Speciality");
    
    
    $.ajax({
        type: "POST",
        url: "/Expertises/InsertExpertise",
        data: '{Speciality: "' + Speciality.val() +   '" }',
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (r) {
            var row = $("#tblExpertise tr:last-child");
            if ($("#tblExpertise tr:last-child span").eq(0).html() != "&nbsp;") {
                row = row.clone();
              //  console.log("Ekkhane aschi");
                //console.log(r.FirstName);
                //console.log(r.BirthDate);
            }
            AppendRow(row, r.ExpertiseID, r.Speciality);
            ExpetiseId.val("");
            Speciality.val("");
            
        }
    });
});


// Edit Event Handler
$("body").on("click", "#tblExpertise .Edit", function () {
    var row = $(this).closest("tr");
    $("td", row).each(function () {
        if ($(this).find("input").length > 0) {
            $(this).find("input").show();
            $(this).find("span").hide();
           // $(this).find("a").hide();
        }
    });
    row.find(".Update").show();
    row.find(".Cancel").show();
    row.find(".Delete").hide();
    $(this).hide();
});



//Update event handler
$("body").on("click", "#tblExpertise .Update", function () {
    var row = $(this).closest("tr");
    $("td", row).each(function () {
        if ($(this).find("input").length > 0) {
            var span = $(this).find("span");
            var input = $(this).find("input");
            var val = input.val();
            //val += "aaaaaaaaaa";
            span.html(val);
            
            span.show();
            span.hide();
            input.hide();
            span.show();
        }
    });
    row.find(".Edit").show();
    row.find(".Delete").show();
    row.find(".Cancel").hide();
    $(this).hide();

    var expertise = {};
    expertise.ExpertiseID = row.find(".ExpertiseId").find("span").html();
    expertise.Speciality = row.find(".Speciality").find("span").html();
    console.log(expertise);
    $.ajax({
        type: "POST",
        url: "/Expertises/UpdateExpertise",
        data: '{expertise:' + JSON.stringify(expertise) + '}',
        contentType: "application/json; charset=utf-8",
        dataType: "json"
    });
});



//Cancel Event Handler
$("body").on("click", "#tblExpertise .Cancel", function () {
    var row = $(this).closest("tr");
    $("td", row).each(function () {
        if ($(this).find("input").length > 0) {
            var span = $(this).find("span");
            var input = $(this).find("input");
            input.val(span.html());
            span.show();
            input.hide();
        }
    });
    row.find(".Edit").show();
    row.find(".Delete").show();
    row.find(".Update").hide();
    $(this).hide();
});





//Delete Event Handler
$("body").on("click", "#tblExpertise .Delete", function () {
    if (confirm("Do you want to delete this row?")) {
        var row = $(this).closest("tr");
        var expertiseId = row.find("span").html();
        $.ajax({
            type: "POST",
            cache: false,
            url: "/Expertises/DeleteExpertise",
            data: '{ExpertiseID: ' + expertiseId + '}',
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                
                if ($("#tblExpertise tr").length > 2) {
                    //debugger;
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


