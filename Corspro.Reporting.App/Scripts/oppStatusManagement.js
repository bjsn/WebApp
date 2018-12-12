(function () {
    $(document).ready(function () {
        var href = window.location.href.split('/');
        var editedRows = new Array();

        var source =
            {
                url: "/Admin/OppStatusList",
                datatype: "json",
                datafields:
                [
                    { name: 'ID', type: 'int' },
                    { name: 'ClientID', type: 'int' },
                    { name: 'OppStatus', type: 'string' },
                    { name: 'Order', type: 'string' },
                    { name: 'OppProbability', type: 'int' },
                    { name: 'DefaultB', type: 'bool' },
                    { name: 'StageType', type: 'string' }
                ],
                //                sortcolumn: 'Order',
                //                sortdirection: 'asc',
                pager: function (pagenum, pagesize, oldpagenum) {
                    // callback called when a page or page size is changed.
                },
                addrow: function (rowid, rowdata, position, commit) {
                    // synchronize with the server - send insert command
                    // call commit with parameter true if the synchronization with the server is successful 
                    //and with parameter false if the synchronization failed.
                    // you can pass additional argument to the commit callback which represents the new ID if it is generated from a DB.
                    commit(true, rowdata.ID);
                },
                deleterow: function (rowid, commit) {
                    // Do something with the result
                    commit(true);
                },
                updaterow: function (rowid, newdata, commit) {
                    // that function is called after each edit.
                    //                    var rowindex = $("#jqxgrid").jqxGrid('getrowboundindexbyid', rowid);          
                    //                    editedRows.push({ index: rowindex, data: newdata });
                    //                    // synchronize with the server - send update command
                    // call commit with parameter true if the synchronization with the server is successful 
                    // and with parameter false if the synchronization failder.
                    commit(true);

                }
            };

        // initialize the input fields.
        $("#txtOppStatus").jqxInput({ theme: 'arctic' });
        $("#txtOrder").jqxNumberInput({ width: '220px', height: '23px', spinButtons: false, theme: 'arctic', min: 0, max: 100, digits: 2, decimalDigits: 0, inputMode: 'simple' });
        $("#txtProbability").jqxNumberInput({ width: '190px', height: '23px', spinButtons: false, theme: 'arctic', min: 0, max: 100, digits: 3, decimalDigits: 0, inputMode: 'simple' });
        $("#statusDefault").jqxCheckBox({ theme: 'arctic' });

        var srcStageType = [
                    "Open",
                    "Won",
                    "Lost"
		        ];

        var srcAllStageType = [
                    "Open",
                    "Won",
                    "Lost"
		        ];

        var stageTypeDA = new $.jqx.dataAdapter(srcStageType);

        $("#stageType").jqxDropDownList({ source: stageTypeDA, selectedIndex: 0, width: '220', height: '23', autoDropDownHeight: true });

        $("#txtOppStatus").width(220);
        $("#txtOppStatus").height(23);

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
                enablehover: false,
                columnsresize: true,
                selectionmode: 'checkbox',
                editmode: 'click',
                columns: [
                    { text: 'ID', datafield: 'ID', hidden: true },
                    { text: 'Opportunity Stage', columntype: 'textbox', datafield: 'OppStatus', width: '20%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer },
                    { text: 'Order', columntype: 'textbox', datafield: 'Order', width: '20%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer,
                        validation: function (cell, value) {
                            if (value < 0 || value > 100) {
                                return { result: false, message: "Order should be in the 0-100 interval" };
                            }
                            return true;
                        }
                        //                        ,
                        //                        createeditor: function (row, cellvalue, editor) {
                        //                            editor.jqxNumberInput({ decimalDigits: 0, digits: 3, inputMode: 'simple', spinButtons: false });
                        //                        }
                    },
                    { text: 'Probability', columntype: 'textbox', datafield: 'OppProbability', width: '20%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer,
                        validation: function (cell, value) {
                            if (value < 0 || value > 100) {
                                return { result: false, message: "Probability should be in the 0-100 interval" };
                            }
                            return true;
                        }
                        //                        ,
                        //                        createeditor: function (row, cellvalue, editor) {
                        //                            editor.jqxNumberInput({ decimalDigits: 0, digits: 3, inputMode: 'simple', spinButtons: false });
                        //                        }
                    },
                    { text: 'Default', columntype: 'checkbox', datafield: 'DefaultB', width: '19%', renderer: columnsrenderer },
                    { text: 'Stage Type', columntype: 'dropdownlist', datafield: 'StageType', width: '19%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer,
                        createeditor: function (row, value, editor) {
                            editor.jqxDropDownList({ source: srcAllStageType });
                        }
                    }
                ]
            });

        var formmodified = 0;
        $("#jqxgrid").on('cellbeginedit', function (event) {
            formmodified = 1;
        });

        var display = '';
        var dField = '';
        var newValue = null;
        var oldValue = null;
        var uId = 0;

        function updateGrid(event) {
            if (display == dField && newValue != oldValue) {
                $.ajax({
                    url: '/Admin/UpdateOppStatusField',
                    data: {
                        cdfId: uId,
                        field: dField,
                        newValue: newValue
                    },
                    type: 'PUT',
                    success: function (result) {
                        if (result.MgUserId >= 0) {
                            formmodified = 0;
                        }
                        else {
                            $("#jqxgrid").jqxGrid('setcellvalue', event.args.rowindex, dField, oldValue);
                            alert("There is already an existing opportunity status with that order.  Please check your data.");
                            formmodified = 0;
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
        }

        $("#jqxgrid").on('cellendedit', function (event) {
            var datarow = $("#jqxgrid").jqxGrid('getrowdata', event.args.rowindex);
            var column = $("#jqxgrid").jqxGrid('getcolumn', event.args.datafield);
            oldValue = null;
            newValue = String(event.args.value);
            display = column.displayfield;
            dField = event.args.datafield;
            uId = datarow.ID;

            if (dField == 'OppStatus') {
                oldValue = datarow.OppStatus;
            }
            if (dField == 'Order') {
                oldValue = String(datarow.Order);
            }
            if (dField == 'OppProbability') {
                oldValue = String(datarow.OppProbability);
            }
            if (dField == 'StageType') {
                oldValue = String(datarow.StageType);
            }
            var existsView = -1;
            var existsUid = -1;
            var rowIDs = new Array();
            var rowUIDs = new Array();
            $("#jqxgrid").jqxGrid('beginupdate');
            //            if (dField == 'DefaultB') {
            //                oldValue = String(event.args.oldvalue);
            //                existsView = event.args.rowindex;
            //                var newuId = uId;

            //                var allRows = $("#jqxgrid").jqxGrid('getrows');
            //                for (var m = 0; m < allRows.length; m++) {
            //                    if (String(allRows[m].DefaultB) == 'true' && event.args.rowindex != allRows[m].uid) {
            //                        //                        existsView = allRows[m].uid;
            //                        //                        existsUid = allRows[m].ID;
            //                        rowUIDs.push(allRows[m].uid);
            //                        rowIDs.push(allRows[m].ID);
            //                    }
            //                }

            //                if (oldValue == 'true') {
            //                    $("#jqxgrid").jqxGrid('setcellvalue', existsView, dField, event.args.checked);
            //                    newValue = 'true';
            //                }

            //                if (rowUIDs.length > 0) {
            //                    for (var n = 0; n < rowUIDs.length; n++) {
            //                        $("#jqxgrid").jqxGrid('setcellvalue', rowUIDs[n], dField, event.args.unchecked);
            //                        uId = rowIDs[n];
            //                        newValue = 'false';
            //                        oldValue = 'true';
            //                        updateGrid(null);
            //                    }
            //                    for (var n = 0; n < rowUIDs.length; n++) {
            //                        for (var m = 0; m < allRows.length; m++) {
            //                            if (allRows[m].uid == rowUIDs[n]) {
            //                                allRows[m].DefaultB = false;
            //                            }
            //                        }
            //                    }
            //                    uId = newuId;
            //                    newValue = 'true';
            //                    oldValue = String(event.args.oldvalue);
            //                }
            //            }

            updateGrid(event);
            $("#jqxgrid").jqxGrid('endupdate');
        });

        var _wasCalled = false;

        function updateDefault() {
            if (!_wasCalled) {
                _wasCalled = true;
                var rowids = new Array();
                var allrows = $("#jqxgrid").jqxGrid('getrows');
                var lowest, lowestB, lowestId, lowestBId;
                if (allrows.length > 0) {
                    lowest = 99;
                    lowestB = 99;
                    lowestId = allrows[0].ID;
                    lowestBId = allrows[0].ID;
                }
                for (var m = 0; m < allrows.length; m++) {
                    if (String(allrows[m].DefaultB) == 'true') {
                        rowids.push(allrows[m].id);
                        if (allrows[m].Order < lowest) {
                            lowest = allrows[m].Order;
                            lowestId = allrows[m].ID;
                        }
                    }
                    if (allrows[m].Order < lowestB) {
                        lowestB = allrows[m].Order;
                        lowestBId = allrows[m].ID;
                    }
                }

                if (rowids.length > 1) {
                    //if (confirm('You selected more than one default status. If you leave, the lowest selected order will be saved as the only default')) {
                    $.ajax({
                        url: '/Admin/UpdateOppStatusDefault',
                        async: false,
                        data: {
                            cdfId: lowestId
                        },
                        type: 'PUT',
                        success: function (result) {
                            if (result.MgUserId >= 0) {
                                formmodified = 0;
                            }
                        },
                        error: function (result) {
                            alert(result);
                        }
                    });
                    //                }
                    //                else {
                    //                    return false;
                    //                }
                }

                if (allrows.length > 0 && rowids.length == 0) {
                    // if (confirm('You have to select at least one default status. If you leave, the lowest order will be saved as default')) {
                    $.ajax({
                        url: '/Admin/UpdateOppStatusDefault',
                        async: false,
                        data: {
                            cdfId: lowestBId
                        },
                        type: 'PUT',
                        success: function (result) {
                            if (result.MgUserId >= 0) {
                                formmodified = 0;
                            }
                        },
                        error: function (result) {
                            alert(result);
                        }
                    });
                    //                }
                    //                else {
                    //                    return false;
                    //                }
                }
            }
        };

        function confirmExit() {
            if (formmodified == 1) {
                updateGrid(null);
                formmodified == 0
                //return true;
                //return "New information not saved. Do you wish to leave the page?";
            }

            var rowids = new Array();
            var allrows = $("#jqxgrid").jqxGrid('getrows');
            for (var m = 0; m < allrows.length; m++) {
                if (String(allrows[m].DefaultB) == 'true') {
                    rowids.push(allrows[m].id);
                }
            }

            updateDefault();
        };

        window.onbeforeunload = confirmExit;

        window.onunload = updateDefault;

        $("#delrowbutton").jqxButton({ theme: 'arctic' });
        $("#delrowbutton").on('click', function () {
            var rows = $("#jqxgrid").jqxGrid('selectedrowindexes');
            var selectedRecords = new Array();
            var selectedQuoteIds = "";

            if (rows.length > 0) {
                for (var n = 0; n < rows.length; n++) {
                    var row = $("#jqxgrid").jqxGrid('getrowdata', rows[n]);
                    selectedRecords[selectedRecords.length] = row;
                    selectedQuoteIds += row.ID;
                    if (n < (rows.length - 1))
                        selectedQuoteIds += ",";
                }

                if (confirm("Are you sure you want to delete these?")) {
                    $.ajax({
                        url: '/Admin/DeleteOppStatuses',
                        data: { cdfIdList: selectedQuoteIds },
                        type: 'DELETE',
                        success: function (result) {
                            if (result.MgUserId > 0) {
                                var array = result.StrMessage.split(',');
                                var rowIDs = new Array();
                                for (var m = 0; m < rows.length; m++) {
                                    var row = $("#jqxgrid").jqxGrid('getrowdata', rows[m]);
                                    var exists = $.inArray(String(row.ID), array);
                                    if (exists == -1) {
                                        var id = $("#jqxgrid").jqxGrid('getrowid', rows[m]);
                                        rowIDs.push(id);
                                    }
                                }
                                var commit = $("#jqxgrid").jqxGrid('deleterow', rowIDs);
                                $("#jqxgrid").jqxGrid('clearselection');
                                if (result.StrMessage != 'Successful') {
                                    alert("Ids:" + result.StrMessage + " could not be deleted. Please check");
                                }
                            }
                            else {
                                alert(result.StrMessage);
                            }
                        },
                        error: function (result) {
                            alert(result);
                        }
                    });
                } else {
                    return false;
                }
            }
            else {
                alert("Must select at least one opportunity status to delete");
            }
        });

        $("#addrowbutton").jqxButton({ theme: 'arctic' });
        // create new row.
        $("#addrowbutton").on('click', function () {
            var offset = $("#jqxgrid").offset();
            $("#popupWindow").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 30} });
            // get the clicked row's data and initialize the input fields.
            $("#txtOppStatus").val('');
            $("#txtOrder").jqxNumberInput({ decimal: 0 });
            $("#txtProbability").jqxNumberInput({ decimal: 0 });
            $('#statusDefault').jqxCheckBox({ checked: false });
            $("#stageType").jqxDropDownList('selectIndex', 0);

            // show the popup window.
            $("#popupWindow").jqxWindow('open');
        });

        // initialize the popup window and buttons.
        $("#popupWindow").jqxWindow({
            width: 500, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#Cancel"), modalOpacity: 0.3
        });

        $("#Cancel").jqxButton({ theme: 'arctic' });
        $("#Save").jqxButton({ theme: 'arctic' });

        $("#Save").click(function () {
            $("#Save").jqxButton({ disabled: true });
            $("#Cancel").jqxButton({ disabled: true });
            var row = {
                ID: 0,
                OppStatus: $("#txtOppStatus").val(),
                Order: $("#txtOrder").val(),
                OppProbability: $("#txtProbability").val(),
                DefaultB: $('#statusDefault').jqxCheckBox('checked'),
                StageType: $("#stageType").jqxDropDownList('val')
            };
            $.ajax({
                url: '/Admin/AddOppStatus',
                data: row,
                datatype: "application/json",
                type: 'POST',
                success: function (result) {
                    if (result.MgUserId > 0) {
                        //alert("User was added successfully with Id:" + result.MgUserId);
                        var existsView = -1;
                        if (row.DefaultB == true) {
                            var rowIDs = new Array();
                            var rowUIDs = new Array();
                            var allRows = $("#jqxgrid").jqxGrid('getrows');
                            for (var m = 0; m < allRows.length; m++) {
                                if (String(allRows[m].DefaultB) == 'true') {
                                    rowUIDs.push(allRows[m].uid);
                                    rowIDs.push(allRows[m].ID);
                                }
                            }

                            if (rowUIDs.length > 0) {
                                for (var n = 0; n < rowUIDs.length; n++) {
                                    $("#jqxgrid").jqxGrid('setcellvalue', rowUIDs[n], 'DefaultB', false);
                                    uId = rowIDs[n];
                                    newValue = 'false';
                                    oldValue = 'true';
                                    updateGrid(null);
                                }
                            }
                        }

                        row.ID = result.MgUserId;
                        var commit = $("#jqxgrid").jqxGrid('addrow', null, row);
                        $("#popupWindow").jqxWindow('hide');
                        $("#Save").jqxButton({ disabled: false });
                        $("#Cancel").jqxButton({ disabled: false });
                    }
                    else {
                        alert("There is already an existing opportunity status with that order.  Please check your data.");
                        $("#Save").jqxButton({ disabled: false });
                        $("#Cancel").jqxButton({ disabled: false });
                    }
                }
            });
        });
    });
} ()); 