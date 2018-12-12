(function () {
    $(document).ready(function () {
       var href = window.location.href.split('/');

       var usersList= new Array();

       $.ajax({
            url: '/Admin/Managers',
            type: 'GET',
            success: function (result) {
                $.each(result, function( ind, datarow ) {
                    usersList.push({StrMessage: datarow.StrMessage, MgUserId: datarow.MgUserId});
                });
            },
            error: function (result) {
                alert(result);
            }
        });


       var sourceDdl =
                {
                    localdata: usersList,
                    datatype: "array",
                    datafields:
                    [
                        { name: 'StrMessage', type: 'string' },
                        { name: 'MgUserId', type: 'int' }
                    ],
                    id: 'MgUserId'
                };
        
       var manageDdl =
                {
                    url: "/Admin/Managers",
                    datatype: "json",
                    datafields:
                    [
                        { name: 'StrMessage', type: 'string' },
                        { name: 'MgUserId', type: 'int' }
                    ],
                    id: 'MgUserId'
                };

       var dataAdapterDdl = new $.jqx.dataAdapter(sourceDdl, {
            formatData: function (datas) {
                $.extend(datas, {
                    featureClass: "P",
                    style: "full",
                    maxRows: 50
                });
                return datas;
             } 
             , autoBind: true
        });

        var managersAdapter=  new $.jqx.dataAdapter(manageDdl, {
            formatData: function (datas) {
                $.extend(datas, {
                    featureClass: "P",
                    style: "full",
                    maxRows: 50
                });
                return datas;
             } , autoBind: true
        });

        var source =
            {
                url: "/Admin/UserDataGrid",
                datatype: "json",
                datafields:
                [
                    { name: 'UserId', type: 'int' },
                    { name: 'ClientId', type: 'int' },
                    { name: 'LoginID', type: 'string' },
                    { name: 'Password', type: 'string' },
                    { name: 'FirstName', type: 'string' },
                    { name: 'LastName', type: 'string' },
                    { name: 'ManagerUserID', type: 'int' },
                    { name: 'ManagerUserName', value: 'ManagerUserID', values: { source: dataAdapterDdl.records, value: 'MgUserId', name: 'StrMessage' } },
                    { name: 'Administrator', type: 'bool' },
                    { name: 'Manager', type: 'string' },
                    { name: 'EmailSent', type: 'string' }
                ],
                sortcolumn: 'LoginID',
                sortdirection: 'asc',
                pager: function (pagenum, pagesize, oldpagenum) {
                    // callback called when a page or page size is changed.
                },
                addrow: function (rowid, rowdata, position, commit) {
                    // synchronize with the server - send insert command
                    // call commit with parameter true if the synchronization with the server is successful 
                    //and with parameter false if the synchronization failed.
                    // you can pass additional argument to the commit callback which represents the new ID if it is generated from a DB.
                    commit(true, rowdata.UserId);
                },
                deleterow: function (rowid, commit) {
                    // Do something with the result
                    commit(true);
                },
                updaterow: function (rowid, newdata, commit) {
                    // that function is called after each edit.
                    // synchronize with the server - send update command
                    // call commit with parameter true if the synchronization with the server is successful 
                    // and with parameter false if the synchronization failder.
                    commit(true);
                    
                }
            };

        // initialize the input fields.
        $("#userId").jqxInput({ theme: 'arctic', disabled: true });
        $("#loginId").jqxInput({ theme: 'arctic' });
        $("#firstName").jqxInput({ theme: 'arctic' });
        $("#lastName").jqxInput({ theme: 'arctic' });
        $("#password").jqxInput({ theme: 'arctic' });
        $("#passwordConfirm").jqxInput({ theme: 'arctic' });
        $("#setpassword").jqxInput({ theme: 'arctic' });
        $("#setpasswordConfirm").jqxInput({ theme: 'arctic' });

        $("#managerId").jqxDropDownList({ source: managersAdapter, displayMember: "StrMessage", valueMember: "MgUserId", theme: 'arctic', width: 220, height: 23 , autoDropDownHeight: true});

        $("#userId").width(220);
        $("#userId").height(23);
        $("#loginId").width(220);
        $("#loginId").height(23);
        $("#firstName").width(220);
        $("#firstName").height(23);
        $("#lastName").width(220);
        $("#lastName").height(23);
        $("#systemAdmin").jqxCheckBox({ theme: 'arctic' });
        $("#systemAdmin").width(20);
        $("#systemAdmin").height(23);
        $("#password").width(220);
        $("#password").height(23);
        $("#passwordConfirm").width(220);
        $("#passwordConfirm").height(23);
        $("#setpassword").width(220);
        $("#setpassword").height(23);
        $("#setpasswordConfirm").width(220);
        $("#setpasswordConfirm").height(23);

        var dataAdapter = new $.jqx.dataAdapter(source,
                {
                    formatData: function (datas) {
                        $.extend(datas, {
                            featureClass: "P",
                            style: "full",
                            maxRows: 50
                        });
                        return datas;
                    }
                }
            );

        var cellsrenderer = function (row, column, value) {
            return '<div style="font-size: 12px;font-family: Verdana;text-align: left;">' + value + '</div>';
        };
        var columnsrenderer = function (value) {
            return '<div style="font-size: 12px;font-family: Verdana;text-align: center;font-weight: bold">' + value + '</div>';
        };

        function validateEmail(email) { 
            var re = /^(([^<>()[\]\\.,;:\s@\"]+(\.[^<>()[\]\\.,;:\s@\"]+)*)|(\".+\"))@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\])|(([a-zA-Z\-0-9]+\.)+[a-zA-Z]{2,}))$/;
            return re.test(email);
        };

        var managerLst = new Array();

        var editrow = -1;

        // initialize jqxGrid
        $("#jqxgrid").jqxGrid(
            {
                width: '99%',
                height: '99%',
                source: dataAdapter,
                theme: 'arctic',
                editable: true,
                sortable: true,
                altrows: true,
                columnsresize: true,
                selectionmode: 'checkbox',
                enablehover: false,
                editmode: 'click',
                columns: [
                    { text: 'User ID', datafield: 'UserId', hidden: true },
                    { text: 'Login', columntype: 'textbox', datafield: 'LoginID', width: '19%', cellsalign: 'left',
                      validation: function (cell, value) {
                         if (validateEmail(value)) {
                            return true;
                          } else {
                            return { result: false, message: "Email address is not valid" };
                          }
                        }, renderer: columnsrenderer, cellsrenderer: cellsrenderer },
                    { text: 'First Name', columntype: 'textbox', datafield: 'FirstName', width: '19%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer },
                    { text: 'Last Name', columntype: 'textbox', datafield: 'LastName', width: '19%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer },
                    { text: 'Manager', columntype: 'dropdownlist',datafield: 'ManagerUserID', displayfield: 'ManagerUserName',width: '19%', renderer: columnsrenderer, cellsrenderer: cellsrenderer,                    
                        createeditor: function (row, value, editor) {
                            editor.jqxDropDownList({ source: dataAdapterDdl, displayMember: 'StrMessage', valueMember: 'MgUserId' });
                        } 
                    },
                    { text: 'Admin', columntype: 'checkbox', datafield: 'Administrator', width: '5%', renderer: columnsrenderer },
                    { text: 'Email Sent', columntype: 'textbox', datafield: 'EmailSent', width: '17%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer },
                ]
            });

        var formmodified=0;
        $("#jqxgrid").on('cellbeginedit', function (event) {
            var column = event.args.datafield;
            var row = event.args.rowindex;
            if (column == "EmailSent") 
            {
                $("#jqxgrid").jqxGrid('endcelledit', row, column, true);
            }
            else
            {
                formmodified=1;
            }
        });

        var display = '';
        var dField = '';
        var newValue=null;
        var oldValue=null;
        var uId=0;

        function updateGrid(event)
        {
            if (display == dField && newValue != oldValue) {
                $.ajax({
                    url: '/Admin/UpdateUserField',
                    data: {
                        userId: uId,
                        field: dField,
                        newValue: newValue 
                    },
                    type: 'PUT',
                    success: function (result) {
                        formmodified = 0;
                    },
                    error: function (result) {
                        alert(result);
                        event.stopPropagation();
                    }
                });
            }
            else if (display != dField && newValue != oldValue){
                var row = {
                    UserId: uId,
                    ManagerUserID: newValue
                };
                $.ajax({
                    url: '/Admin/UpdateUserManager',
                    data: row,
                    type: 'PUT',
                    success: function (result) {
                        if (result.MgUserId>0)
                        {
                            formmodified = 0;
                            //reloadGrid();
                        }
                        else
                        {
                            alert(result.StrMessage);
                            dataAdapter.dataBind();
                            if (event != null) {
                                event.stopPropagation();
                            }
                        }
                    },
                    error: function (result) {
                        alert(result);
                        if (event != null) {
                                event.stopPropagation();
                            }
                    }
                });      
            }
            else
            {
                formmodified=0;
            }
        };

        $("#jqxgrid").on('cellendedit', function (event) {
            var datarow = $("#jqxgrid").jqxGrid('getrowdata', event.args.rowindex);
            var column = $("#jqxgrid").jqxGrid('getcolumn', event.args.datafield);
            newValue = String(event.args.value);
            oldValue = String(event.args.oldvalue);
            display = column.displayfield;
            dField= event.args.datafield;
            uId= datarow.UserId;
            if (display != dField)
            {
                newValue = event.args.value.value;
                oldValue = event.args.oldvalue.value;
            }
            updateGrid(event);
        });

        

        function confirmExit() {
            if (formmodified == 1) {
                updateGrid(null);
               //return true;
            }
        };

        window.onbeforeunload = confirmExit;
    

        $("#delrowbutton").jqxButton({ theme: 'arctic' });
        $("#delrowbutton").on('click', function () {
            var rows = $("#jqxgrid").jqxGrid('selectedrowindexes');
            var selectedRecords = new Array();
            var selectedQuoteIds = "";

            if (rows.length > 0) {
                for (var m = 0; m < rows.length; m++) {
                    var row = $("#jqxgrid").jqxGrid('getrowdata', rows[m]);
                    selectedRecords[selectedRecords.length] = row;
                    selectedQuoteIds += row.UserId;
                    if (m < (rows.length - 1))
                        selectedQuoteIds += ",";
                }

                if (confirm("Are you sure you want to delete these?")) {
                    $.ajax({
                        url: '/Admin/DeleteUserRows',
                        data: { userIdList: selectedQuoteIds },
                        type: 'DELETE',
                        success: function (result) {
                            var array = result.StrMessage.split(',');
                            var rowIDs = new Array();
                            for (var n = 0; n < rows.length; n++) {
                                var row = $("#jqxgrid").jqxGrid('getrowdata', rows[n]);
                                var exists = $.inArray(String(row.UserId), array);
                                if (exists == -1) {
                                    var id = $("#jqxgrid").jqxGrid('getrowid', rows[n]);
                                    rowIDs.push(id);
                                }
                            }
                            var commit = $("#jqxgrid").jqxGrid('deleterow', rowIDs);
                        },
                        error: function (result) {
                            alert(result);
                        }
                    });
                } else {
                    return false;
                }
            }
            else
            {
                alert("Must select at least one user to delete");
            }
        });

        $("#addrowbutton").jqxButton({ theme: 'arctic' });
        // create new row.
        $("#addrowbutton").on('click', function () {
            var offset = $("#jqxgrid").offset();
            $("#popupWindow").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 30} });
            // get the clicked row's data and initialize the input fields.
            $("#userId").val('');
            $("#loginId").val('');
            $("#firstName").val('');
            $("#lastName").val('');

            $("#managerId").jqxDropDownList('selectIndex', 0);
            $('#systemAdmin').jqxCheckBox({ checked: false });

            $("#password").val('');
            $("#passwordConfirm").val('');

            // show the popup window.
            $("#popupWindow").jqxWindow('open');
        });

        // initialize the popup window and buttons.
        $("#popupWindow").jqxWindow({
            width: 500, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#Cancel"), modalOpacity: 0.3
        });
        $("#popupWindow").on('open', function () {
            $("#loginId").jqxInput('selectAll');
        });

        $("#Passpopup").jqxWindow({
            width: 500, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#passCancel"), modalOpacity: 0.3
        });

        $('#userForm').jqxValidator({
            scroll: false,
            rules: [
                    { input: '#loginId', message: '*', action: 'keyup, blur', rule: 'required' },
                    { input: '#loginId', message: 'Invalid e-mail!', action: 'keyup', rule: 'email' },
                    { input: '#firstName', message: '*', action: 'keyup, blur', rule: 'required' },
                    { input: '#lastName', message: '*', action: 'keyup, blur', rule: 'required' },
                    { input: '#managerId', message: 'Please choose a manager', action: 'keyup, blur, change', rule: function (input, commit) {
                        if (input.val() != "") {
                            return true;
                        } else {
                            return false;
                            }
                        }
                    },
                    { input: "#password", message: "Password is required!", action: 'keyup, blur', rule: 'required' },
                        { input: "#passwordConfirm", message: "Password is required!", action: 'keyup, blur', rule: 'required' },
                        {
                            input: "#passwordConfirm", message: "Passwords should match!", action: 'keyup, blur', rule: function (input, commit) {
                                var firstPassword = $("#password").val();
                                var secondPassword = $("#passwordConfirm").val();
                                return firstPassword == secondPassword;
                            }
                        },],
            theme: 'arctic'
        });

        $('#passForm').jqxValidator({
            scroll: false,
            rules: [
                    { input: "#setpassword", message: "Password is required!", action: 'keyup, blur', rule: 'required' },
                        { input: "#setpasswordConfirm", message: "Password is required!", action: 'keyup, blur', rule: 'required' },
                        {
                            input: "#setpasswordConfirm", message: "Passwords should match!", action: 'keyup, blur', rule: function (input, commit) {
                                var firstPassword = $("#setpassword").val();
                                var secondPassword = $("#setpasswordConfirm").val();
                                return firstPassword == secondPassword;
                            }
                        },],
            theme: 'arctic'
        });

        $("#Cancel").jqxButton({ theme: 'arctic' });
        $("#Save").jqxButton({ theme: 'arctic' });
        $("#passCancel").jqxButton({ theme: 'arctic' });
        $("#passSave").jqxButton({ theme: 'arctic' });
        $("#ResetPassword").jqxButton({ theme: 'arctic' });
        $("#sendEmail").jqxButton({ theme: 'arctic' });
        $("#Generate").jqxButton({ theme: 'arctic' });
        $("#passGenerate").jqxButton({ theme: 'arctic' });

        $("#Generate").click(function () {
            var d = new Date();
            var n = d.getTime();
            $.ajax({
                url: '/Admin/Generate/?version=' + n,
                data: { minLength: 8, maxLength: 10 },
                datatype: "application/json",
                type: 'GET',
                success: function (result) {
                    $("#password").val(result);
                    $("#passwordConfirm").val(result);
                }
            });
        });

        $("#passGenerate").click(function () {
            var d = new Date();
            var n = d.getTime();
            $.ajax({
                url: '/Admin/Generate/?version=' + n,
                data: { minLength: 8, maxLength: 10 },
                datatype: "application/json",
                type: 'GET',
                success: function (result) {
                    $("#setpassword").val(result);
                    $("#setpasswordConfirm").val(result);    
                }
            });
        });

        // update the edited row when the user clicks the 'Save' button.
        $("#Save").click(function () {
            $("#Save").jqxButton({ disabled: true });
            $("#Cancel").jqxButton({ disabled: true });
            var onSuccess = $('#userForm').jqxValidator('validate');
            if (onSuccess) {
                var row = {
                    UserId: $("#userId").val(),
                    LoginID: $("#loginId").val(),
                    FirstName: $("#firstName").val(),
                    LastName: $("#lastName").val(),
                    ManagerUserID: $('#managerId').jqxDropDownList('val'),
                    ManagerUserName: $('#managerId').jqxDropDownList('getSelectedItem').label,
                    Administrator: $('#systemAdmin').jqxCheckBox('checked'),
                    Password: $('#password').val(),
                };
                $.ajax({
                    url: '/Admin/AddUserRow',
                    data: row,
                    datatype: "application/json",
                    type: 'POST',
                    success: function (result) {
                        if(result.MgUserId != -2)
                        {
                            row.UserId = result.MgUserId;
                            var commit = $("#jqxgrid").jqxGrid('addrow', null, row);
                            $("#popupWindow").jqxWindow('hide');
                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        }
                        else
                        {
                            alert("There is already a user record with this email address.  Please check to make sure you have the correct email address.");
                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        }
                    }
                });
            }
        });

        $("#passSave").click(function () {
            $("#passSave").jqxButton({ disabled: true });
            $("#passCancel").jqxButton({ disabled: true });
            var onSuccess = $('#passForm').jqxValidator('validate');
            if (onSuccess) {
                var row = {
                    UserId: $("#passuserId").val(),
                    Password: $('#setpassword').val()
                };
                 $.ajax({
                    url: '/Admin/ResetPasswordRow',
                    data: row,
                    datatype: "application/json",
                    type: 'PUT',
                    success: function (result) {
                         $("#Passpopup").jqxWindow('hide');
                            $("#passSave").jqxButton({ disabled: false });
                            $("#passCancel").jqxButton({ disabled: false });
                    }
                });
            }
        });

        $("#ResetPassword").click(function () {
            var rows = $("#jqxgrid").jqxGrid('selectedrowindexes');
            
            if (rows.length == 1) {
                var offset = $("#jqxgrid").offset();
                $("#Passpopup").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 30} });
                // get the clicked row's data and initialize the input fields.
                var editrow = $("#jqxgrid").jqxGrid('selectedrowindex');
                var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', editrow);

                $("#passuserId").val(dataRecord.UserId);
            
                $("#setpassword").val('');
                $("#setpasswordConfirm").val('');

                // show the popup window.
                $("#Passpopup").jqxWindow('open');
            }
            else
            {
                alert("Please select only one user to reset password");
            }
        });

        $("#sendEmail").click(function () {
            var rows = $("#jqxgrid").jqxGrid('selectedrowindexes');
            
            if (rows.length > 0) {
                if (confirm("The selected users will be sent an email with their login and password.  Please press Ok to send the emails or press the Cancel button to cancel.")) {
                    var selectedRecords = new Array();
                    var selectedQuoteIds = "";

                    for (var m = 0; m < rows.length; m++) {
                        var row = $("#jqxgrid").jqxGrid('getrowdata', rows[m]);
                        selectedRecords[selectedRecords.length] = row;
                        selectedQuoteIds += row.UserId;
                        if (m < (rows.length - 1))
                            selectedQuoteIds += ",";
                    }

                    $.ajax({
                        url: '/Admin/SetRequestedEmail',
                        data: { userIdList: selectedQuoteIds },
                        type: 'PUT',
                        success: function (result) {
                            var array = result.StrMessage.split(',');
                            var rowIDs = new Array();
                            var datas = new Array();
                            for (var n = 0; n < rows.length; n++) {
                                var row = $("#jqxgrid").jqxGrid('getrowdata', rows[n]);
                                var exists = $.inArray(String(row.UserId), array);
                                if (exists == -1) {
                                    var id = $("#jqxgrid").jqxGrid('getrowid', rows[n]);
                                    rowIDs.push(id);
                                    row.EmailSent="Requested";
                                    datas.push(row);
                                }
                            }
                            var commit = $("#jqxgrid").jqxGrid('updaterow', rowIDs, datas); 
                            $("#jqxgrid").jqxGrid('clearselection');
                        },
                        error: function (result) {
                            alert(result);
                        }
                    });
                }
            }
            else
            {
                alert("Please select at least one user to send an email");
            }
        });
    });
} ()); 