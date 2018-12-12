(function () {
    $(document).ready(function () {
        var href = window.location.href.split('/');
        var editedRows = new Array();

        var source =
            {
                url: "/Admin/ClientDefinedFieldList",
                datatype: "json",
                datafields:
                [
                    { name: 'ClientDefinedFieldID', type: 'int' },
                    { name: 'ClientId', type: 'int' },
                    { name: 'Table', type: 'string' },
                    { name: 'Field', type: 'string' },
                    { name: 'ColumnHeader', type: 'string' },
                    { name: 'Format', type: 'string' },
                    { name: 'SDARangeName', type: 'string' }
                ],
                sortcolumn: 'Table',
                sortdirection: 'asc',
                pager: function (pagenum, pagesize, oldpagenum) {
                    // callback called when a page or page size is changed.
                },
                addrow: function (rowid, rowdata, position, commit) {
                    // synchronize with the server - send insert command
                    // call commit with parameter true if the synchronization with the server is successful 
                    //and with parameter false if the synchronization failed.
                    // you can pass additional argument to the commit callback which represents the new ID if it is generated from a DB.
                    commit(true, rowdata.ClientDefinedFieldID);
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
        var sourceTable = [
                    "Opportunity",
                    "Quote"
		        ];

        $("#gridId").jqxDropDownList({ source: sourceTable, selectedIndex: 1, width: '220', height: '23', autoDropDownHeight: true });

        var sourceCDF = [
                    "ClientDefinedTotal1",
                    "ClientDefinedTotal2",
                    "ClientDefinedTotal3",
                    "ClientDefinedTotal4",
                    "ClientDefinedTotal5",
                    "ClientDefinedTotal6",
                    "ClientDefinedTotal7",
                    "ClientDefinedTotal8",
                    "ClientDefinedTotal9",
                    "ClientDefinedTotal10",
                    "ClientDefinedText1",
                    "ClientDefinedText2",
                    "ClientDefinedText3",
                    "ClientDefinedText4",
                    "ClientDefinedText5"
		        ];

        var sourceTexts = [
                    "ClientDefinedText1",
                    "ClientDefinedText2",
                    "ClientDefinedText3",
                    "ClientDefinedText4",
                    "ClientDefinedText5"
		        ];

        var CDFDataAdapter = new $.jqx.dataAdapter(sourceTexts);

        $("#clientDefinedFieldId").jqxDropDownList({ source: CDFDataAdapter, selectedIndex: 1, width: '220', height: '23', autoDropDownHeight: true });

        $("#sdaRangeName").jqxInput({ theme: 'arctic' });
        $("#columnHeader").jqxInput({ theme: 'arctic' });

        var sourceAllFormat = [
                    "Currency",
                    "Number",
                    "Text"
		        ];

        var sourceFormat = [
                    "Currency",
                    "Number"
		        ];

        var sourceText = [
                    "Text"
		        ];

        var formatDataAdapter = new $.jqx.dataAdapter(sourceText);

        $("#format").jqxDropDownList({ source: formatDataAdapter, selectedIndex: 1, width: '220', height: '23', autoDropDownHeight: true });

        $("#gridId").bind('select', function (event) {
            if (event.args) {
                var value = event.args.item.value;
                var items = $("#clientDefinedFieldId").jqxDropDownList('getItems');
                if (value.indexOf("Opportunity") > -1 && items.length > 5) {
                    $("#clientDefinedFieldId").jqxDropDownList({ disabled: false, selectedIndex: -1 });
                    CDFDataAdapter = new $.jqx.dataAdapter(sourceTexts);
                    $("#clientDefinedFieldId").jqxDropDownList({ source: CDFDataAdapter });
                    $("#clientDefinedFieldId").jqxDropDownList('selectIndex', 0);
                    formatDataAdapter = new $.jqx.dataAdapter(sourceText);
                    $("#format").jqxDropDownList({ source: formatDataAdapter });
                    $("#format").jqxDropDownList('selectIndex', 0);
                }
                if (value.indexOf("Quote") > -1 && items.length < 10) {
                    $("#clientDefinedFieldId").jqxDropDownList({ disabled: false, selectedIndex: -1 });
                    CDFDataAdapter = new $.jqx.dataAdapter(sourceCDF);
                    $("#clientDefinedFieldId").jqxDropDownList({ source: CDFDataAdapter });
                    $("#clientDefinedFieldId").jqxDropDownList('selectIndex', 0);
                    formatDataAdapter = new $.jqx.dataAdapter(sourceAllFormat);
                    $("#format").jqxDropDownList({ source: formatDataAdapter });
                    $("#format").jqxDropDownList('selectIndex', 0);
                }
            }
        });

        $("#sdaRangeName").width(220);
        $("#sdaRangeName").height(23);
        $("#columnHeader").width(220);
        $("#columnHeader").height(23);

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
                columnsresize: true,
                enablehover: false,
                selectionmode: 'checkbox',
                editmode: 'click',
                columns: [
                    { text: 'ClientDefinedFieldID', datafield: 'ClientDefinedFieldID', hidden: true },
                    { text: 'Entity', columntype: 'dropdownlist', datafield: 'Table', width: '19%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer,
                        createeditor: function (row, value, editor) {
                            editor.jqxDropDownList({ source: sourceTable });
                        }
                    },
                    { text: 'Field', columntype: 'dropdownlist', datafield: 'Field', width: '19%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer,
                        initeditor: function (row, cellvalue, editor, celltext, cellwidth, cellheight) {
                            var country = $('#jqxgrid').jqxGrid('getcellvalue', row, "Table");
                            var city = editor.val();
                            var nsource = sourceTexts;
                            switch (country) {
                                case "Opportunity":
                                    nsource = sourceTexts;
                                    editor.jqxDropDownList({ autoDropDownHeight: true, source: sourceTexts });
                                    break;
                                case "Quote":
                                    nsource = sourceCDF;
                                    editor.jqxDropDownList({ autoDropDownHeight: true, source: sourceCDF });
                                    break;
                            };

                            var exists = $.inArray(city, nsource);
                            if (exists != -1) {
                                var index = nsource.indexOf(city);
                                editor.jqxDropDownList('selectIndex', index);
                            }
                        }
                    },
                    { text: 'SDA Range Name', columntype: 'textbox', datafield: 'SDARangeName', width: '20%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer },
                    { text: 'Column Header', columntype: 'textbox', datafield: 'ColumnHeader', width: '21%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer },
                    { text: 'Format', columntype: 'dropdownlist', datafield: 'Format', width: '19%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: cellsrenderer,
                        createeditor: function (row, value, editor) {
                            editor.jqxDropDownList({ source: sourceAllFormat });
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
                    url: '/Admin/UpdateCDFField',
                    data: {
                        cdfId: uId,
                        field: dField,
                        newValue: newValue
                    },
                    type: 'PUT',
                    success: function (result) {
                        if (result.MgUserId != -2) {
                            formmodified = 0;
                        }
                        else {
                            if (dField == 'Table' || dField == 'Field') {
                                dataAdapter.dataBind();
                                //$("#jqxgrid").jqxGrid('setcellvalue', event.args.rowindex, dField, oldValue);
                            }
                            alert("There is already a record with this grid and field combination.  Please check your data.");
                            formmodified = 0;
                            if (event != null) {
                                //event.stopPropagation();
                            }
                        }
                    },
                    error: function (result) {
                        alert(result);
                        if (event != null) {
                            dataAdapter.dataBind();
                            //event.stopPropagation();
                        }
                    }
                });
            }
        };

        $("#jqxgrid").on('cellendedit', function (event) {
            var datarow = $("#jqxgrid").jqxGrid('getrowdata', event.args.rowindex);

            var column = $("#jqxgrid").jqxGrid('getcolumn', event.args.datafield);
            oldValue = String(event.args.oldvalue);
            newValue = String(event.args.value);
            display = column.displayfield;
            dField = event.args.datafield;
            uId = datarow.ClientDefinedFieldID;
            var idx = event.args.rowindex;

            if (dField == 'Table' && newValue == 'Opportunity' && datarow.Field.indexOf("ClientDefinedTotal") > -1) {
                alert("This field is not valid for the opportunity record");
                //$("#jqxgrid").jqxGrid('setcellvalue', idx, dField, oldValue);
                formmodified = 0;
                dataAdapter.dataBind();
                //event.stopPropagation();
            }
            else if ((dField == 'Field' && ((newValue.indexOf("ClientDefinedTotal") > -1 && datarow.Format == "Text")
                  || (newValue.indexOf("ClientDefinedText") > -1 && datarow.Format != "Text")))
                  || (dField == 'Format' && ((datarow.Field.indexOf("ClientDefinedTotal") > -1 && newValue == "Text")
                  || (datarow.Field.indexOf("ClientDefinedText") > -1 && newValue != "Text")))) {

                alert("Error - Illegal format assignment. Please review your data");
                dataAdapter.dataBind();
                //$("#jqxgrid").jqxGrid('setcellvalue', idx, dField, oldValue);
                formmodified = 0;
                //event.stopPropagation();
            }
            else {
                updateGrid(event);
            }
        });

        function confirmExit() {
            if (formmodified == 1) {
                updateGrid(null);
                //return true;
                //return "New information not saved. Do you wish to leave the page?";
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
                    selectedQuoteIds += row.ClientDefinedFieldID;
                    if (m < (rows.length - 1))
                        selectedQuoteIds += ",";
                }

                if (confirm("Are you sure you want to delete these?")) {
                    $.ajax({
                        url: '/Admin/DeleteCDFs',
                        data: { cdfIdList: selectedQuoteIds },
                        type: 'DELETE',
                        success: function (result) {
                            if (result.MgUserId > 0) {
                                var array = result.StrMessage.split(',');
                                var rowIDs = new Array();
                                for (var m = 0; m < rows.length; m++) {
                                    var row = $("#jqxgrid").jqxGrid('getrowdata', rows[m]);
                                    var exists = $.inArray(String(row.ClientDefinedFieldID), array);
                                    if (exists == -1) {
                                        var id = $("#jqxgrid").jqxGrid('getrowid', rows[m]);
                                        rowIDs.push(id);
                                    }
                                }
                                var commit = $("#jqxgrid").jqxGrid('deleterow', rowIDs);
                                if (result.StrMessage != 'Successful') {
                                    alert("Ids:" + result.StrMessage + " could not be deleted");
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
                alert("Must select at least one client defined field to delete");
            }
        });

        $("#addrowbutton").jqxButton({ theme: 'arctic' });
        // create new row.
        $("#addrowbutton").on('click', function () {
            var offset = $("#jqxgrid").offset();
            $("#popupWindow").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 30} });
            // get the clicked row's data and initialize the input fields.
            $("#sdaRangeName").val('');
            $("#columnHeader").val('');

            $("#gridId").jqxDropDownList('selectIndex', 0);
            $("#clientDefinedFieldId").jqxDropDownList('selectIndex', 0);
            $("#format").jqxDropDownList('selectIndex', 2);


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
                ClientDefinedFieldID: 0,
                Table: $("#gridId").jqxDropDownList('val'),
                Field: $("#clientDefinedFieldId").jqxDropDownList('val'),
                ColumnHeader: $("#columnHeader").val(),
                Format: $("#format").jqxDropDownList('val'),
                SDARangeName: $('#sdaRangeName').val()
            };

            if ((row.Field.indexOf("ClientDefinedTotal") > -1 && row.Format == "Text") || (row.Field.indexOf("ClientDefinedText") > -1 && row.Format != "Text")) {
                alert("Wrong format assignment. Please review your data");
                $("#Save").jqxButton({ disabled: false });
                $("#Cancel").jqxButton({ disabled: false });
            }
            else {
                $.ajax({
                    url: '/Admin/AddClientDefinedField',
                    data: row,
                    datatype: "application/json",
                    type: 'POST',
                    success: function (result) {
                        if (result.MgUserId != -2) {
                            //alert("User was added successfully with Id:" + result.MgUserId);
                            row.ClientDefinedFieldID = result.MgUserId;
                            var commit = $("#jqxgrid").jqxGrid('addrow', null, row);
                            $("#popupWindow").jqxWindow('hide');
                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        }
                        else {
                            alert("There is already a record with this grid and field combination.  Please check your data.");
                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        }
                    }
                });
            }
        });
    });
} ()); 