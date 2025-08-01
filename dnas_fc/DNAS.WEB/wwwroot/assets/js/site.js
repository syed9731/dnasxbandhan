
(function ($) {
    "use strict";
    // For desktop nav control
    $(function () {
        $(".navbar-toggler-desktop").on("click", function () {
            $("body").toggleClass("sidebar-icon-only");
        });
        // For mobile nav control
        $(".navbar-toggler-mobile").on("click", function () {
            $(".sidebar-offcanvas").toggleClass("active");
        });
        // For OTP control
        $(".digit-group")
            .find("input")
            .each(function () {
                $(this).attr("maxlength", 1);
                $(this).on("keyup", function (e) {
                    let parent = $($(this).parent());

                    if (e.keyCode === 8 || e.keyCode === 37) {
                        let prev = parent.find("input#" + $(this).data("previous"));

                        if (prev.length) {
                            $(prev).select();
                        }
                    } else if (
                        (e.keyCode >= 48 && e.keyCode <= 57) ||
                        (e.keyCode >= 65 && e.keyCode <= 90) ||
                        (e.keyCode >= 96 && e.keyCode <= 105) ||
                        e.keyCode === 39
                    ) {
                        let next = parent.find("input#" + $(this).data("next"));

                        if (next.length) {
                            $(next).select();
                        } else {
                            if (parent.data("autosubmit")) {
                                parent.submit();
                            }
                        }
                    }
                });
            });
    });

    jQuery(".category-select").change(function () {
        let categoryname = $("#CategoryId option:selected").text();
        $("#CategoryName").val(categoryname);
        $("#confirmapprover").html("");
        $("#ApproverIdList").val("");
        $("#ExpensesIncurredAtid").val("");
        $("#NatureOfExpensesId").val("");
        $("#totalamount").val("");
        if ($("#CategoryId option:selected").text() == "Financial") {
            getExpensesIncurredAt($(this).val());
            $(".amount-box").show();
            $("#CapitalExpenditure").val("");
            $("#OperationalExpenditure").val("");
            $("#approversearch").hide();
            $('#approveradd').hide();
            $('#recomendedapproverdiv').show();
        }
        else {
            $("#ExpensesIncurredAtid").val("");
            $("#NatureOfExpensesId").val("");
            $("#CapitalExpenditure").val('0');
            $("#OperationalExpenditure").val('0');
            $(".amount-box").hide();
            $("#approversearch").val('').show();            
            $('#recomendedapproverdiv').hide();
            ClearRecomendedApprover();
        }
    });

    function getExpensesIncurredAt(id) {
        $.getJSON("/note/fetchExpensesIncurredAt", { idd: id }, function (data) {
            if (data != null) {
                let items = '';
                items += "<option value=''>Select</option>";
                $.each(data, function (i, j) {
                    items += "<option value='" + data[i].value + "'>" + data[i].key + "</option>";
                });
                $("#ExpensesIncurredAtid").html(items);
            }
        });
    }

    $("#ExpensesIncurredAtid").on('change', function () {
        let expIncurredAtId = $("#ExpensesIncurredAtid").val();
        $.getJSON("/note/fetchNatureOfExpense", { idd: expIncurredAtId }, function (data) {
            if (data != null) {
                let items = '';
                items += "<option value=''>Select</option>";
                $.each(data, function (i, j) {
                    items += "<option value='" + data[i].value + "'>" + data[i].key + "</option>";
                });
                $("#NatureOfExpensesId").html(items);
            }
        });
    });
    $("#approversearch").on('keyup', function () {        
        let searchkey = $("#approversearch").val();
        let categoryname = $("#CategoryId option:selected").text();
        let noe = $("#NatureOfExpensesId option:selected").val();
        let totalamount = $("#totalamount").val();
        $.getJSON("/note/fetchApproverlist", { idd: searchkey, catname: categoryname, netofexp: noe, totAmt: totalamount }, function (data) {            
            if (data != null) {
                let item = '';
                $.each(data, function (i, j) {                    
                    let strng = Decryptdata($("#gstbdg").val(), $("#ApproverIdList").val());
                    let apprlist = strng.split(',');
                    //if (!apprlist.includes(data[i].value)) {
                    if (apprlist.filter(function (elem) { return elem == data[i].value; }) == '') {
                        item += "<li style='cursor:pointer' class='createapprover' onclick='getapproverr(" + data[i].value + ")'><p class='underpara'><span id='approverlistid_" + data[i].value + "' class='underspan'>" + data[i].key + "</span></p></li>";
                    }
                });
                $("#approverlist").html(item);
            }
        });
        $("#approverlist").css("display", "block");
    });
    $("#recomendedapproversearch").on('keyup', function () {
        let searchkey = $("#recomendedapproversearch").val();
        if ($("#ExpensesIncurredAtid").val() === '') {
            $("#errormsg").text("Please Select Expenses Incurred At");
            $("#error_popup").modal('show');
        }
        if ($("#NatureOfExpensesId").val() == 'null' || $("#NatureOfExpensesId").val() == '') {
            $("#errormsg").text("Please Select Nature Of Expense");
            $("#error_popup").modal('show');            
        }
        if ($("#CapitalExpenditure").val() == '') {            
            $("#errormsg").text("Please Provide Capital Expenditure");
            $("#error_popup").modal('show');
        }
        if ($("#OperationalExpenditure").val() == '') {
            $("#errormsg").text("Please Provide Operational Expenditure");
            $("#error_popup").modal('show');
        }
        if ($("#ExpensesIncurredAtid").val() != '' && $("#NatureOfExpensesId").val() != 'null' && $("#NatureOfExpensesId").val() != '' && $("#CapitalExpenditure").val() != '' && $("#OperationalExpenditure").val()!='') {
        $.getJSON("/note/FetchRecomendedApproverlist", { idd: searchkey }, function (data) {
                if (data != null) {
                    let item = '';                    
                    
                    $.each(data, function (i, j) {                        
                        let recomendedstrng = Decryptdata($("#gstbdg").val(), $("#RecomendedApproverIdList").val());  
                        let recomendedapprlist = recomendedstrng.split(','); 
                        let strng = Decryptdata($("#gstbdg").val(), $("#ApproverIdList").val());
                        let apprlist = strng.split(',');
                        if (recomendedapprlist.filter(function (elem) { return elem == data[i].value; }) == '' && apprlist.filter(function (elem) { return elem == data[i].value; }) == '') {
                            item += "<li style='cursor:pointer' class='createrecomendapprover' onclick='getrecomendedapproverr(" + data[i].value + ")'><p class='underpara'><span id='recomendedapproverlistid_" + data[i].value + "' class='underspan'>" + data[i].key + "</span></p></li>";
                        }                        
                    });
                    $("#recomendedapproverlist").html(item);
                }
            });
            $("#recomendedapproverlist").css("display", "block");
            $("#error_popup").modal('hide');
        }
        
    });
    
    $("#approverremove").click(function () {
        $("#selectedapproverwithadd").css("display", "none");
        $("#approversearch").css("display", "block");
        let startindex = $("#ApproverIdList").val().indexOf($("#firstapproveruserid").val());
        if (startindex > 1) {
            let str = ',' + $("#firstapproveruserid").val();
            $("#ApproverIdList").val($("#ApproverIdList").val().replace(str, ""));
        }
        else {
            let str = $("#firstapproveruserid").val();
            $("#ApproverIdList").val($("#ApproverIdList").val().replace(str, ""));
        }
    });
    

    $("#CapitalExpenditure").blur(function () {
        ClearRecomendedApprover();
        let title = $("#CapitalExpenditure").val();
        title = title.split(",").join("");
        if (title.match(/^\d+(\.\d+)?$/))   //check input number or not
        {
            let num= parseFloat($("#OperationalExpenditure").val() == '' ? 0.0 : ($("#OperationalExpenditure").val()).split(",").join("")) + parseFloat(title);
            $("#totalamount").val(formatNumberForTotal(num));
            if ($("#NoteId").val() != '') {
                $.getJSON("/note/UpdateNoteCapex", { noteid: $("#NoteId").val(), capex: $("#CapitalExpenditure").val(), totalamt: $("#totalamount").val() }, function (data) {
                    
                    sucesstoast();
                    $('.sucessmsg').html('Saved to draft');
                });
            }
            else {
                $.ajax({
                    type: "GET",
                    url: "/note/InsertNoteCapex",
                    datatype: 'json',
                    data: { opex: $("#OperationalExpenditure").val() },
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data != null) {
                            $("#NoteId").val(data.noteId);
                            sucesstoast();
                            $('.sucessmsg').html('Saved to draft');
                        }
                    },
                    error: function (textStatus, errorThrown) {
                        console.log(textStatus);
                    }
                });
            }
        }
        else {
            $("#CapitalExpenditure").val(title.slice(0, -1));    //remove last character input
        }
    });
    $("#OperationalExpenditure").blur(function () {
        ClearRecomendedApprover();
        let title = $("#OperationalExpenditure").val();
        title = title.split(",").join("");
        if (title.match(/^\d+(\.\d+)?$/))   //check input number or not
        {
            let num = parseFloat(title) + parseFloat($("#CapitalExpenditure").val() == '' ? 0 : ($("#CapitalExpenditure").val()).split(",").join(""));

            $("#totalamount").val(formatNumberForTotal(num));
            if ($("#NoteId").val() != '') {
                $.getJSON("/note/UpdateNoteOpex", { noteid: $("#NoteId").val(), opex: $("#OperationalExpenditure").val(), totalamt: $("#totalamount").val() }, function (data) {
                    
                    sucesstoast();
                    $('.sucessmsg').html('Saved to draft');
                });
            }
            else {
                $.ajax({
                    type: "GET",
                    url: "/note/InsertNoteOpex",
                    datatype: 'json',
                    data: { opex: $("#OperationalExpenditure").val() },
                    contentType: "application/json; charset=utf-8",
                    success: function (data) {
                        if (data != null) {
                            $("#NoteId").val(data.noteId);
                            sucesstoast();
                            $('.sucessmsg').html('Saved to draft');
                        }
                    },
                    error: function (textStatus, errorThrown) {
                        console.log(textStatus);
                    }
                });
            }
        }
        else {
            $("#OperationalExpenditure").val(title.slice(0, -1));    //remove last character input
        }
    });
    $("#CapitalExpenditure").on("keyup", function () {
        let title = $("#CapitalExpenditure").val();
        title = parseInt(title);
        if (title.toString().length > 12) {
            $("#CapitalExpenditure").val(title.slice(0, -1));
        }        
    });
    $("#OperationalExpenditure").on("keyup", function () {
        let title = $("#OperationalExpenditure").val();
        title = parseInt(title);
        if (title.length > 12) {
            $("#OperationalExpenditure").val(title.slice(0, -1));
        }
    });
    function formatNumberForTotal(num) {
        var decimalPart = (num % 1).toFixed(2).substring(1);
        num = parseInt(num);
        num = num.toString();
        let newstr = '';
        if (num.length > 10) {
            newstr = num.slice(0, -10) + ',';
            num = num.slice(num.length - 10, num.length);
        }
        if (num.length > 7) {
            if (newstr == '') {
                newstr = num.slice(0, -7) + ',';
            }
            else {
                newstr = newstr + num.slice(0, -7) + ',';
            }
            num = num.slice(num.length - 7, num.length);
        }
        if (num.length > 5) {
            if (newstr == '') {
                newstr = num.slice(0, -5) + ',';
            }
            else {
                newstr = newstr + num.slice(0, -5) + ',';
            }
            num = num.slice(num.length - 5, num.length);
        }
        if (num.length > 3) {
            if (newstr == '') {
                newstr = num.slice(0, -3) + ',';
            }
            else {
                newstr = newstr + num.slice(0, -3) + ',';
            }
            num = num.slice(num.length - 3, num.length);
        }
        if (num.length <= 3) {
            newstr = newstr + num;
        }
        newstr = newstr + decimalPart;

        return newstr;
    }
    $("#notepreview").on("click", function () {
        if ($("#notetitle").val() != '' && $("#summernote").val() != '') {
            $("#staticBackdropLabel").text($("#notetitle").val());
            $("#previewnotebody").html($("#summernote").val());
            $("#view_popup").modal('show');
        }
        else {
            $("#errormsg").text("Please fill note title and note description");
            $("#error_popup").modal('show');
        }
    });

    $("#btnsavetemplate").on("click", function () {
        if ($("#notetitle").val() != '' && $("#summernote").val() != '' && $("#CategoryId").val() != '') {
            $("#view_TemplateName").modal('show');
        }
        else {
            $("#errormsg").text("Please fill title, category and note description fields");
            $("#error_popup").modal('show');
        }
    });
    $("#notetitle").on("blur", function () {
        if ($("#NoteId").val() != '') {
            $.getJSON("/note/UpdateNoteTitle", { noteid: $("#NoteId").val(), NoteTitle: $("#notetitle").val() }, function (data) {
                
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: "/note/InsertNoteTitle",
                datatype: 'json',
                data: { NoteTitle: $("#notetitle").val() },
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data != null) {
                        $("#NoteId").val(data.noteId);
                       
                    }
                },
                error: function (textStatus, errorThrown) {
                    console.log(textStatus);
                }
            });            
        }
    });
    $("#CategoryId").on("change", function () {
        if ($("#NoteId").val() != '') {
            
            $.getJSON("/note/UpdateNoteCategory", { noteid: $("#NoteId").val(), CategoryId: $("#CategoryId").val() }, function (data) {
                
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: "/note/InsertCategory",
                datatype: 'json',
                data: { cat: $("#CategoryId").val() },
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data != null) {
                        $("#NoteId").val(data.noteId);
                       
                    }
                },
                error: function (textStatus, errorThrown) {
                    console.log(textStatus);
                }
            });
        }
     });
    $("#ExpensesIncurredAtid").on("change", function () {
        ClearRecomendedApprover();
        if ($("#NoteId").val() != '') {
            $.getJSON("/note/UpdateNoteExpenseIncurredAt", { noteid: $("#NoteId").val(), ExpAtId: $("#ExpensesIncurredAtid").val() }, function (data) {
                
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: "/note/InsertNoteExpensesIncurredAt",
                datatype: 'json',
                data: { exp: $("#ExpensesIncurredAtid").val() },
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data != null) {
                        $("#NoteId").val(data.noteId);
                    }
                },
                error: function (textStatus, errorThrown) {
                    console.log(textStatus);
                }
            });
        }
    });
    $("#NatureOfExpensesId").on("change", function () {
        ClearRecomendedApprover();
        if ($("#NoteId").val() != '') {
            $.getJSON("/note/UpdateNoteNetureOfExpense", { noteid: $("#NoteId").val(), expId: $("#NatureOfExpensesId").val() }, function (data) {
                
            });
        }
        else {
            $.ajax({
                type: "GET",
                url: "/note/InsertNatureOfExp",
                datatype: 'json',
                data: { cat: $("#NatureOfExpensesId").val() },
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data != null) {
                        $("#NoteId").val(data.noteId);
                    }
                },
                error: function (textStatus, errorThrown) {
                    console.log(textStatus);
                }
            });
        }
    });    

    $("#btnSaveTemp").on("click", function () {
        if ($("#TemplateName").val() != '' && $("#CategoryId").val() != '' && $("#notetitle").val() != '' && $("#summernote").val()) {
            

            let request = { userid: $("#UserId").val(), catid: $("#CategoryId").val(), tempname: $("#TemplateName").val(), notetitle: $("#notetitle").val(), notebody: $("#summernote").val() };
            let headers = {};
            headers['RequestVerificationToken'] = $('input[name="__RequestVerificationToken"]').val();
            $.ajax({
                type: "post",
                cache: false,
                url: "/note/SaveTemplate",
                data: JSON.stringify(request),
                dataType: "json",
                headers: headers,
                contentType: "application/json; charset=utf-8",
                success: function (data) {
                    if (data == "success") {
                        $("#successmsg").text("The template has been saved successfully.");
                        $("#success_popup").modal('show');
                        $("#view_TemplateName").modal('hide');
                        $('#TemplateName').val('');
                    }
                    else {
                        $("#errormsg").text("Template not save.");
                        $("#error_popup").modal('show');
                    }
                },
                error: function (result) {
                    console.log("No Connection to server");
                },
            });



        }
        else {
            $("#errormsg").text("Please insert mandatory fields");
            $("#error_popup").modal('show');
        }
    });
    function ClearRecomendedApprover() {
        $("#RecomendedApproverIdList").val("");
        $("#confirmrecomendedapprover").html('');
        $("#confirmrecomendedapprover").hide();
        $("#recomendedapproversearch").show();
    }

   
})(jQuery);

function sucesstoast() {
    var toastEl = document.getElementById('sucesstoast');
    var sucesstoast = new bootstrap.Toast(toastEl, {
        autohide: true,
        delay: 5000 // Adjust the delay in milliseconds
    });
    sucesstoast.show();
}

function errortoast() {
    var toastEl = document.getElementById('erroralert');
    var Errortoast = new bootstrap.Toast(toastEl, {
        autohide: true,
        delay: 5000 // Adjust the delay in milliseconds
    });
    Errortoast.show();
}

