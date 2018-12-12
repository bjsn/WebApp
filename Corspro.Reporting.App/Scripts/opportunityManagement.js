(function () {
    $(document).ready(function () {

        //Block the UI
        $.blockUI({
            message: $('#divLoadding'),
            css: {
                top: ($(window).height() - 400) / 2 + 'px',
                left: ($(window).width() - 400) / 2 + 'px',
                width: '400px'
            }
        });

        var SaveAsName = '';

        var first = true;

        var dateArray = new Array();
        dateArray.push("THIS_MONTH");

        var combinations = "THIS_MONTH|Open/";
        var combinationsOld = "THIS_MONTH|Open/";

        var stageArray = new Array();
        stageArray.push("Open");

        var stages = 'Open';

        var source = null;
        var initialDate = finalDate = dateJ = new Date();

        var yP = dateJ.getFullYear(), mP = dateJ.getMonth();
        initialDate = new Date(yP, mP, 1);

        if (mP < 11) {
            finalDate = new Date(yP, mP + 1, 0);
        }
        if (mP == 11) {
            yP = yP + 1, mP = 0;
            finalDate = new Date(yP, mP, 0);
        }

        $("#dateTitle").text('Showing Open opportunities closing This Month - \'');
        $("#viewTitle").text('Standard\' view');
        $("#drInitialDate").jqxDateTimeInput({ height: '16.9px', width: '100px', formatString: 'MM/dd/yyyy', disabled: true });
        $("#drFinalDate").jqxDateTimeInput({ height: '16.9px', width: '100px', formatString: 'MM/dd/yyyy', disabled: true });

        $("#CD_DateRange").jqxCheckBox({ height: '20px', theme: 'arctic' });

        $("#CD_DateRange").off('change').on('change', function (event) {
            var checked = event.args.checked;
            if (checked) {
                $("#drInitialDate").jqxDateTimeInput({ disabled: false });
                $("#drFinalDate").jqxDateTimeInput({ disabled: false });
            }
            else {
                $("#drInitialDate").jqxDateTimeInput({ disabled: true });
                $("#drFinalDate").jqxDateTimeInput({ disabled: true });
            }
        });

        var strCustomFilter = "Custom=Open;THIS_MONTH;&";

        if (sessvars.records) {
            source = { localdata: sessvars.records,
                datatype: "array",
                datafields:
                                [
                                    { name: 'OppID', type: 'int' },
                                    { name: 'OppStatusId', type: 'int' },
                                    { name: 'OppStatusName', type: 'string' },
                                    { name: 'StageType', type: 'string' },
                                    { name: 'OppProbability', type: 'int' },
                                    { name: 'OppCloseDate', type: 'date' },
                                    { name: 'OppOwner', type: 'string' },
                                    { name: 'OppOwnerName', type: 'string' },
                                    { name: 'OppName', type: 'string' },
                                    { name: 'CompanyName', type: 'string' },
                                    { name: 'CRMOppID', type: 'string' },
                                    { name: 'QuotedAmount', type: 'float' },
                                    { name: 'QuotedCost', type: 'float' },
                                    { name: 'QuotedMargin', type: 'float' },
                                    { name: 'NumofQuotes', type: 'int' },
                                    { name: 'Manager', type: 'string' },
                                    { name: 'CreateDT', type: 'date' },
                                    { name: 'ClientDefinedTotal1', type: 'float' },
                                    { name: 'ClientDefinedTotal2', type: 'float' },
                                    { name: 'ClientDefinedTotal3', type: 'float' },
                                    { name: 'ClientDefinedTotal4', type: 'float' },
                                    { name: 'ClientDefinedTotal5', type: 'float' },
                                    { name: 'ClientDefinedTotal6', type: 'float' },
                                    { name: 'ClientDefinedTotal7', type: 'float' },
                                    { name: 'ClientDefinedTotal8', type: 'float' },
                                    { name: 'ClientDefinedTotal9', type: 'float' },
                                    { name: 'ClientDefinedTotal10', type: 'float' },
                                    { name: 'ClientDefinedText1', type: 'string' },
                                    { name: 'ClientDefinedText2', type: 'string' },
                                    { name: 'ClientDefinedText3', type: 'string' },
                                    { name: 'ClientDefinedText4', type: 'string' },
                                    { name: 'ClientDefinedText5', type: 'string' }
                                ],
                id: 'OppID',
                sortcolumn: 'OppName',
                sortdirection: 'asc',
                addrow: function (rowid, rowdata, position, commit) {
                    // synchronize with the server - send insert command
                    // call commit with parameter true if the synchronization with the server is successful 
                    //and with parameter false if the synchronization failed.
                    // you can pass additional argument to the commit callback which represents the new ID if it is generated from a DB.
                    commit(true, rowdata.OppID);
                },
                deleterow: function (rowid, commit) {
                    // Do something with the result
                    commit(true);
                },
                updaterow: function (rowid, newdata, commit) {
                    // synchronize with the server - send update command 
                    commit(true);
                }
            };

            if (sessvars.customFilter) {
                strCustomFilter = sessvars.customFilter
                var title = "Showing ";
                var arr = strCustomFilter.substring(0, strCustomFilter.length - 1).split('=');

                var filterList = arr[1].split(';');

                var filterdata0 = filterList[0].split('/');
                stageArray = new Array();
                $('#S_Open').jqxCheckBox({ checked: false });
                $('#S_Won').jqxCheckBox({ checked: false });
                $('#S_Lost').jqxCheckBox({ checked: false });

                stages = '';

                for (var m = 0; m < filterdata0.length; m++) {
                    stageArray.push(filterdata0[m]);
                    var lastChar = title.slice(-1);
                    if (lastChar == "n" || lastChar == "t") {
                        title = title + "/";
                        stages = stages + "/";
                    }
                    if (filterdata0[m] == "Open") {
                        title = title + "Open";
                        stages = stages + "Open";
                        $('#S_Open').jqxCheckBox({ checked: true });
                    }
                    if (filterdata0[m] == "Won") {
                        title = title + "Won";
                        stages = stages + "Won";
                        $('#S_Won').jqxCheckBox({ checked: true });
                    }
                    if (filterdata0[m] == "Lost") {
                        title = title + "Lost";
                        stages = stages + "Lost";
                        $('#S_Lost').jqxCheckBox({ checked: true });
                    }
                }

                //filterStageType();

                title = title + " opportunities closing ";

                var filterdata = filterList[1].split('/');
                var initialDateT = new Date();
                var finalDateT = new Date();
                initialDate = new Date(1900, 01, 01);
                finalDate = new Date(1900, 01, 01);
                var tFirstDay = new Date(1900, 01, 01);
                var tLastDay = new Date(1900, 01, 01);

                $('#CD_ThisMonth').jqxCheckBox({ checked: false });
                $('#CD_LastMonth').jqxCheckBox({ checked: false });
                $('#CD_NextMonth').jqxCheckBox({ checked: false });
                $('#CD_ThisQuarter').jqxCheckBox({ checked: false });
                $('#CD_LastQuarter').jqxCheckBox({ checked: false });
                $('#CD_NextQuarter').jqxCheckBox({ checked: false });
                $('#CD_DateRange').jqxCheckBox({ checked: false });

                for (var j = 0; j < filterdata.length; j++) {
                    var lastChar = title.slice(-1);
                    if (lastChar == "h" || lastChar == "r") {
                        title = title + "/";
                    }
                    if (filterdata[j] == "THIS_MONTH") {
                        var y = dateJ.getFullYear(), m = dateJ.getMonth();
                        initialDateT = new Date(y, m, 1);
                        if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                            initialDate = initialDateT;
                        }

                        if (m < 11) {
                            finalDateT = new Date(y, m + 1, 0);
                        }
                        if (m == 11) {
                            y = y + 1, m = 0;
                            finalDateT = new Date(y, m, 0);
                        }
                        if (finalDateT > finalDate) {
                            finalDate = finalDateT;
                        }
                        title = title + "This Month";
                        $('#CD_ThisMonth').jqxCheckBox({ checked: true });
                    }
                    if (filterdata[j] == "LAST_MONTH") {
                        var y = dateJ.getFullYear(), m = dateJ.getMonth();
                        finalDateT = new Date(y, m, 0);
                        if (finalDateT > finalDate) {
                            finalDate = finalDateT;
                        }

                        if (m == 0) {
                            y = y - 1, m = 11;
                            initialDateT = new Date(y, m, 1);
                        }
                        else {
                            initialDateT = new Date(y, m - 1, 1);
                        }
                        if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                            initialDate = initialDateT;
                        }
                        title = title + "Last Month";
                        $('#CD_LastMonth').jqxCheckBox({ checked: true });
                    }
                    if (filterdata[j] == "NEXT_MONTH") {
                        var ly = y = dateJ.getFullYear(), lm = m = dateJ.getMonth();
                        if (m < 11) {
                            initialDateT = new Date(y, m + 1, 1);
                        }
                        else {
                            y = y + 1, m = 1;
                            initialDateT = new Date(y, m, 1);
                        }
                        if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                            initialDate = initialDateT;
                        }

                        if (lm < 10) {
                            finalDateT = new Date(ly, lm + 2, 0);
                        }
                        if (lm == 10) {
                            ly = ly + 1, lm = 0;
                            finalDateT = new Date(ly, lm, 0);
                        }
                        if (lm == 11) {
                            ly = ly + 1, lm = 1;
                            finalDateT = new Date(ly, lm, 0);
                        }
                        if (finalDateT > finalDate) {
                            finalDate = finalDateT;
                        }
                        title = title + "Next Month";
                        $('#CD_NextMonth').jqxCheckBox({ checked: true });
                    }
                    if (filterdata[j] == "THIS_QUARTER") {
                        var y = dateJ.getFullYear(), m = dateJ.getMonth();
                        if (m < 3) {
                            initialDateT = InitFQ;
                            finalDateT = FinFQ;
                        }
                        if (m >= 3 && m < 6) {
                            initialDateT = InitSQ;
                            finalDateT = FinSQ;
                        }
                        if (m >= 6 && m < 9) {
                            initialDateT = InitTQ;
                            finalDateT = FinTQ;
                        }
                        if (m >= 9) {
                            initialDateT = InitFFQ;
                            finalDateT = FinFFQ;
                        }

                        if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                            initialDate = initialDateT;
                        }
                        if (finalDateT > finalDate) {
                            finalDate = finalDateT;
                        }
                        title = title + "This Quarter";
                        $('#CD_ThisQuarter').jqxCheckBox({ checked: true });
                    }
                    if (filterdata[j] == "LAST_QUARTER") {
                        var y = dateJ.getFullYear(), m = dateJ.getMonth();
                        if (m < 3) {
                            var yyQ = y - 1;
                            InitFFQ = new Date(yyQ, 9, 1);
                            FinFFQ = new Date(y, 0, 0);
                            initialDateT = InitFFQ;
                            finalDateT = FinFFQ;
                        }
                        if (m >= 3 && m < 6) {
                            initialDateT = InitFQ;
                            finalDateT = FinFQ;
                        }
                        if (m >= 6 && m < 9) {
                            initialDateT = InitSQ;
                            finalDateT = FinSQ;
                        }
                        if (m >= 9) {
                            initialDateT = InitTQ;
                            finalDateT = FinTQ;
                        }

                        if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                            initialDate = initialDateT;
                        }
                        if (finalDateT > finalDate) {
                            finalDate = finalDateT;
                        }
                        title = title + "Last Quarter";
                        $('#CD_LastQuarter').jqxCheckBox({ checked: true });
                    }
                    if (filterdata[j] == "NEXT_QUARTER") {
                        var y = dateJ.getFullYear(), m = dateJ.getMonth();
                        if (m < 3) {
                            initialDateT = InitSQ;
                            finalDateT = FinSQ;
                        }
                        if (m >= 3 && m < 6) {
                            initialDateT = InitTQ;
                            finalDateT = FinTQ;
                        }
                        if (m >= 6 && m < 9) {
                            initialDateT = InitFFQ;
                            finalDateT = FinFFQ;
                        }
                        if (m >= 9) {
                            var yyQ = y + 1;
                            InitFQ = new Date(yyQ, 0, 1);
                            FinFQ = new Date(yyQ, 3, 0);
                            initialDateT = InitFQ;
                            finalDateT = FinFQ;
                        }

                        if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                            initialDate = initialDateT;
                        }
                        if (finalDateT > finalDate) {
                            finalDate = finalDateT;
                        }
                        title = title + "Next Quarter";
                        $('#CD_NextQuarter').jqxCheckBox({ checked: true });
                    }
                    if (filterdata[j].substring(0, 4) == "DATE") {
                        var dranges = filterdata[j].split('|');
                        initialDateT = new Date(dranges[1]);
                        finalDateT = new Date(dranges[2]);

                        if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                            initialDate = initialDateT;
                        }
                        if (finalDateT > finalDate) {
                            finalDate = finalDateT;
                        }

                        title = title + "Date Range";
                        $('#CD_DateRange').jqxCheckBox({ checked: true });
                        $("#drInitialDate").jqxDateTimeInput('setDate', initialDateT);
                        $("#drFinalDate").jqxDateTimeInput('setDate', finalDateT);
                    }
                }

                $("#dateTitle").text(title + " - \'");
                $("#viewTitle").text(sessvars.viewTitle);
            }
        }
        else {
            source =
            {
                url: "/Home/OpportunityDataByDateRange?t=" + Math.random(),
                data: { initialDate: initialDate.toISOString(), finalDate: finalDate.toISOString(), stages: 'Open' },
                datatype: "json",
                datafields:
                [
                    { name: 'OppID', type: 'int' },
                    { name: 'OppStatusId', type: 'int' },
                    { name: 'OppStatusName', type: 'string' },
                    { name: 'StageType', type: 'string' },
                    { name: 'OppProbability', type: 'int' },
                    { name: 'OppCloseDate', type: 'date' },
                    { name: 'OppOwner', type: 'string' },
                    { name: 'OppOwnerName', type: 'string' },
                    { name: 'OppName', type: 'string' },
                    { name: 'CompanyName', type: 'string' },
                    { name: 'CRMOppID', type: 'string' },
                    { name: 'QuotedAmount', type: 'float' },
                    { name: 'QuotedCost', type: 'float' },
                    { name: 'QuotedMargin', type: 'float' },
                    { name: 'NumofQuotes', type: 'int' },
                    { name: 'Manager', type: 'string' },
                    { name: 'CreateDT', type: 'date' },
                    { name: 'ClientDefinedTotal1', type: 'float' },
                    { name: 'ClientDefinedTotal2', type: 'float' },
                    { name: 'ClientDefinedTotal3', type: 'float' },
                    { name: 'ClientDefinedTotal4', type: 'float' },
                    { name: 'ClientDefinedTotal5', type: 'float' },
                    { name: 'ClientDefinedTotal6', type: 'float' },
                    { name: 'ClientDefinedTotal7', type: 'float' },
                    { name: 'ClientDefinedTotal8', type: 'float' },
                    { name: 'ClientDefinedTotal9', type: 'float' },
                    { name: 'ClientDefinedTotal10', type: 'float' },
                    { name: 'ClientDefinedText1', type: 'string' },
                    { name: 'ClientDefinedText2', type: 'string' },
                    { name: 'ClientDefinedText3', type: 'string' },
                    { name: 'ClientDefinedText4', type: 'string' },
                    { name: 'ClientDefinedText5', type: 'string' }
                ],
                id: 'OppID',
                sortcolumn: 'OppName',
                sortdirection: 'asc',

                addrow: function (rowid, rowdata, position, commit) {
                    // synchronize with the server - send insert command
                    // call commit with parameter true if the synchronization with the server is successful 
                    //and with parameter false if the synchronization failed.
                    // you can pass additional argument to the commit callback which represents the new ID if it is generated from a DB.
                    commit(true, rowdata.OppID);
                },
                deleterow: function (rowid, commit) {
                    // Do something with the result
                    commit(true);
                },
                updaterow: function (rowid, newdata, commit) {
                    // synchronize with the server - send update command 
                    commit(true);
                }
            };
        }

        var filterCloseDate = function () {
            var datefiltergroup = new $.jqx.filter();
            var operator = 0;

            var filtervalue = initialDate.toISOString();
            var filtercondition = 'GREATER_THAN_OR_EQUAL';
            var filter4 = datefiltergroup.createfilter('datefilter', filtervalue, filtercondition);

            filtervalue = finalDate.toISOString();
            filtercondition = 'LESS_THAN_OR_EQUAL';
            var filter5 = datefiltergroup.createfilter('datefilter', filtervalue, filtercondition);

            datefiltergroup.addfilter(operator, filter4);
            datefiltergroup.addfilter(operator, filter5);

            $("#jqxgrid").jqxGrid('removefilter', "OppCloseDate");
            // apply the filters.
            $("#jqxgrid").jqxGrid('applyfilters');

            $("#jqxgrid").jqxGrid('addfilter', "OppCloseDate", datefiltergroup);
            $("#jqxgrid").jqxGrid('applyfilters');
        };

        var filterStageType = function () {
            var datefiltergroup = new $.jqx.filter();
            var operator = 1;

            if (stageArray.length == 0) {
                stageArray.push(" ");
            }

            for (var j = 0; j < stageArray.length; ++j) {
                var filtervalue = stageArray[j];
                var filtercondition = 'EQUAL';
                var filter4 = datefiltergroup.createfilter('stringfilter', filtervalue, filtercondition);

                datefiltergroup.addfilter(operator, filter4);
            }

            $("#jqxgrid").jqxGrid('removefilter', "StageType");
            // apply the filters.
            $("#jqxgrid").jqxGrid('applyfilters');

            $("#jqxgrid").jqxGrid('addfilter', "StageType", datefiltergroup);
            $("#jqxgrid").jqxGrid('applyfilters');

        };

        var rebuildFilter = function (savedFilter, viewId) {
            var title = "Showing ";
            var arr = savedFilter.split('=');

            var filterList = arr[1].split(';');

            var filterdata0 = filterList[0].split('/');
            stageArray = new Array();
            $('#S_Open').jqxCheckBox({ checked: false });
            $('#S_Won').jqxCheckBox({ checked: false });
            $('#S_Lost').jqxCheckBox({ checked: false });

            stages = '';

            for (var m = 0; m < filterdata0.length; m++) {
                stageArray.push(filterdata0[m]);
                var lastChar = title.slice(-1);
                if (lastChar == "n" || lastChar == "t") {
                    title = title + "/";
                    stages = stages + "/";
                }
                if (filterdata0[m] == "Open") {
                    title = title + "Open";
                    stages = stages + "Open";
                    $('#S_Open').jqxCheckBox({ checked: true });
                }
                if (filterdata0[m] == "Won") {
                    title = title + "Won";
                    stages = stages + "Won";
                    $('#S_Won').jqxCheckBox({ checked: true });
                }
                if (filterdata0[m] == "Lost") {
                    title = title + "Lost";
                    stages = stages + "Lost";
                    $('#S_Lost').jqxCheckBox({ checked: true });
                }
            }

            //filterStageType();

            title = title + " opportunities closing ";

            var filterdata = filterList[1].split('/');
            var initialDateT = new Date();
            var finalDateT = new Date();
            initialDate = new Date(1900, 01, 01);
            finalDate = new Date(1900, 01, 01);
            var tFirstDay = new Date(1900, 01, 01);
            var tLastDay = new Date(1900, 01, 01);

            $('#CD_ThisMonth').jqxCheckBox({ checked: false });
            $('#CD_LastMonth').jqxCheckBox({ checked: false });
            $('#CD_NextMonth').jqxCheckBox({ checked: false });
            $('#CD_ThisQuarter').jqxCheckBox({ checked: false });
            $('#CD_LastQuarter').jqxCheckBox({ checked: false });
            $('#CD_NextQuarter').jqxCheckBox({ checked: false });
            $('#CD_DateRange').jqxCheckBox({ checked: false });

            for (var j = 0; j < filterdata.length; j++) {
                var lastChar = title.slice(-1);
                if (lastChar == "h" || lastChar == "r") {
                    title = title + "/";
                }
                if (filterdata[j] == "THIS_MONTH") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    initialDateT = new Date(y, m, 1);
                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }

                    if (m < 11) {
                        finalDateT = new Date(y, m + 1, 0);
                    }
                    if (m == 11) {
                        y = y + 1, m = 0;
                        finalDateT = new Date(y, m, 0);
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "This Month";
                    $('#CD_ThisMonth').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "LAST_MONTH") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    finalDateT = new Date(y, m, 0);
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }

                    if (m == 0) {
                        y = y - 1, m = 11;
                        initialDateT = new Date(y, m, 1);
                    }
                    else {
                        initialDateT = new Date(y, m - 1, 1);
                    }
                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    title = title + "Last Month";
                    $('#CD_LastMonth').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "NEXT_MONTH") {
                    var ly = y = dateJ.getFullYear(), lm = m = dateJ.getMonth();
                    if (m < 11) {
                        initialDateT = new Date(y, m + 1, 1);
                    }
                    else {
                        y = y + 1, m = 1;
                        initialDateT = new Date(y, m, 1);
                    }
                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }

                    if (lm < 10) {
                        finalDateT = new Date(ly, lm + 2, 0);
                    }
                    if (lm == 10) {
                        ly = ly + 1, lm = 0;
                        finalDateT = new Date(ly, lm, 0);
                    }
                    if (lm == 11) {
                        ly = ly + 1, lm = 1;
                        finalDateT = new Date(ly, lm, 0);
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "Next Month";
                    $('#CD_NextMonth').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "THIS_QUARTER") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    if (m < 3) {
                        initialDateT = InitFQ;
                        finalDateT = FinFQ;
                    }
                    if (m >= 3 && m < 6) {
                        initialDateT = InitSQ;
                        finalDateT = FinSQ;
                    }
                    if (m >= 6 && m < 9) {
                        initialDateT = InitTQ;
                        finalDateT = FinTQ;
                    }
                    if (m >= 9) {
                        initialDateT = InitFFQ;
                        finalDateT = FinFFQ;
                    }

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "This Quarter";
                    $('#CD_ThisQuarter').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "LAST_QUARTER") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    if (m < 3) {
                        var yyQ = y - 1;
                        InitFFQ = new Date(yyQ, 9, 1);
                        FinFFQ = new Date(y, 0, 0);
                        initialDateT = InitFFQ;
                        finalDateT = FinFFQ;
                    }
                    if (m >= 3 && m < 6) {
                        initialDateT = InitFQ;
                        finalDateT = FinFQ;
                    }
                    if (m >= 6 && m < 9) {
                        initialDateT = InitSQ;
                        finalDateT = FinSQ;
                    }
                    if (m >= 9) {
                        initialDateT = InitTQ;
                        finalDateT = FinTQ;
                    }

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "Last Quarter";
                    $('#CD_LastQuarter').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "NEXT_QUARTER") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    if (m < 3) {
                        initialDateT = InitSQ;
                        finalDateT = FinSQ;
                    }
                    if (m >= 3 && m < 6) {
                        initialDateT = InitTQ;
                        finalDateT = FinTQ;
                    }
                    if (m >= 6 && m < 9) {
                        initialDateT = InitFFQ;
                        finalDateT = FinFFQ;
                    }
                    if (m >= 9) {
                        var yyQ = y + 1;
                        InitFQ = new Date(yyQ, 0, 1);
                        FinFQ = new Date(yyQ, 3, 0);
                        initialDateT = InitFQ;
                        finalDateT = FinFQ;
                    }

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "Next Quarter";
                    $('#CD_NextQuarter').jqxCheckBox({ checked: true });
                }
                if (filterdata[j].substring(0, 4) == "DATE") {
                    var dranges = filterdata[j].split('|');
                    initialDateT = new Date(dranges[1]);
                    finalDateT = new Date(dranges[2]);

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }

                    title = title + "Date Range";
                    $('#CD_DateRange').jqxCheckBox({ checked: true });
                    $("#drInitialDate").jqxDateTimeInput('setDate', initialDateT);
                    $("#drFinalDate").jqxDateTimeInput('setDate', finalDateT);
                }
            }

            $("#dateTitle").text(title + " - \'");

            loadArchived();
            //filterCloseDate();
        };

        var hideColumns = function (xjqxgrid) {
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal1');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal2');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal3');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal4');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal5');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal6');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal7');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal8');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal9');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedTotal10');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedText1');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedText2');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedText3');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedText4');
            $(xjqxgrid).jqxGrid('hidecolumn', 'ClientDefinedText5');
        };

        var loadDefault = function () {
            $.ajax({
                type: 'GET',
                url: '/Home/LoadDefaultUserView?t=' + Math.random(),
                success: function (response) {
                    if (response != "") {
                        var arr = response.split('&');
                        if (arr.length > 1) {
                            response = JSON.parse(arr[1]);
                        }
                        else {
                            response = JSON.parse(response);
                        }
                        //first = 0;

                        // IF YOU alert(response) YOU SHOULD GET AN [object Object] AND NOT A STRING. IF YOU GET A STRING SUCH HAS "{"width":"100%","height":400..." IT IS WRONG.														
                        $("#jqxgrid").jqxGrid('loadstate', response);

                        showCDFOpps("#jqxgrid");

                        $("#viewTitle").text('Default\' view');

                        strCustomFilter = arr[0] + '&';

                        if (arr.length > 1) {
                            rebuildFilter(arr[0], 0);
                        }
                        $("#menu_top").jqxDropDownList('selectIndex', -1);

                    }
                    else {
                        $("#menu_top").jqxDropDownList('selectIndex', -1);
                    }
                },
                error: function (result) {
                    alert(result);
                }
            });
        };


        var CDFDisplayed = null;
        var qCDFDisplayed = null;

        var getCDFOpps = function (xtable) {
            var d = new Date();
            var n = d.getTime();
            $.ajax({
                type: 'GET',
                url: '/Admin/GetClientDefinedFields/?version=' + n,
                data: { table: xtable },
                datatype: "application/json",
                success: function (response) {
                    if (xtable == "Opportunity") {
                        CDFDisplayed = response;
                    }
                    else {
                        qCDFDisplayed = response;
                    }
                },
                error: function (result) {
                    alert(result);
                }
            });
        };

        getCDFOpps("Opportunity");

        var showCDFOpps = function (xjqxgrid) {
            hideColumns(xjqxgrid);
            var listCDF = null;
            if (xjqxgrid == "#jqxgrid") {
                listCDF = CDFDisplayed;
            }
            else {
                listCDF = qCDFDisplayed;
            }
            $.each(listCDF, function (idx, obj) {
                $(xjqxgrid).jqxGrid('setcolumnproperty', obj.Field, 'text', obj.ColumnHeader);
                $(xjqxgrid).jqxGrid('showcolumn', obj.Field);
                if (obj.Format != '') {
                    $(xjqxgrid).jqxGrid('setcolumnproperty', obj.Field, 'cellsformat', obj.Format);
                }
            });
        };

        ///////////////////////////////////////////////////
        var addDefaultfilter = function () {
            var datefiltergroup = new $.jqx.filter();
            var operator = 0;
            var today = new Date();

            var nextMonth = new Date();

            nextMonth.setDate((today.getDate() + 30));

            var filtervalue = today;
            var filtercondition = 'GREATER_THAN_OR_EQUAL';
            var filter4 = datefiltergroup.createfilter('datefilter', filtervalue, filtercondition);

            filtervalue = nextMonth;
            filtercondition = 'LESS_THAN_OR_EQUAL';
            var filter5 = datefiltergroup.createfilter('datefilter', filtervalue, filtercondition);

            datefiltergroup.addfilter(operator, filter4);
            datefiltergroup.addfilter(operator, filter5);

            $("#jqxgrid").jqxGrid('addfilter', 'range', datefiltergroup);
            $("#jqxgrid").jqxGrid('applyfilters');
        }
        ///////////////////////////////////////////////////

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

        var dateFilters = [
                { value: 'EQUAL', label: 'Equal' },
                { value: 'NOT_EQUAL', label: 'Not Equal' },
                { value: 'LESS_THAN', label: 'Less Than' },
                { value: 'LESS_THAN_OR_EQUAL', label: 'Less Than or Equal' },
                { value: 'GREATER_THAN', label: 'Greater Than' },
                { value: 'GREATER_THAN_OR_EQUAL', label: 'Greater Than or Equal' },
                { value: 'NULL', label: 'Null' },
                { value: 'NOT_NULL', label: 'Not Null' }
            ];
        var dateFilterSource =
            {
                datatype: "array",
                datafields: [
                     { name: 'label', type: 'string' },
                     { name: 'value', type: 'string' }
                 ],
                localdata: dateFilters
            };
        var dateFilterAdapter = new $.jqx.dataAdapter(dateFilterSource, {
            autoBind: true
        });


        var CondOps = [
                { value: 0, label: 'And' },
                { value: 1, label: 'Or' }
            ];
        var CondOpSource =
            {
                datatype: "array",
                datafields: [
                     { name: 'label', type: 'string' },
                     { name: 'value', type: 'int' }
                 ],
                localdata: CondOps
            };
        var CondOpsAdapter = new $.jqx.dataAdapter(CondOpSource, {
            autoBind: true
        });


        var ranges = [
                 { value: "LastMonth", label: "Last Month" },
                 { value: "ThisMonth", label: "This Month" },
                 { value: "NextMonth", label: "Next Month" },
                 { value: "None", label: "Date Range" }
            ];
        var rangesSource =
            {
                datatype: "array",
                datafields: [
                     { name: 'label', type: 'string' },
                     { name: 'value', type: 'string' }
                 ],
                localdata: ranges
            };
        var rangesAdapter = new $.jqx.dataAdapter(rangesSource, {
            autoBind: true
        });

        var cellsrenderer = function (row, column, value) {
            return '<div style="font-size: 12px;font-family: Verdana;text-align: center;">' + value + '</div>';
        };
        var columnsrenderer = function (value) {
            return '<div style="font-size: 12px;font-family: Verdana;text-align: center;font-weight: bold">' + value + '</div>';
        };

        var editrow = -1;

        // initialize jqxGrid
        $("#jqxgrid").jqxGrid({
            width: '99%',
            height: '100%',
            source: dataAdapter,
            theme: 'arctic',
            groupable: true,
            autoloadstate: false,
            autosavestate: false,
            filterable: true,
            autoshowfiltericon: true,
            sortable: true,
            altrows: true,
            columnsresize: true,
            columnsreorder: true,
            showaggregates: true,
            showstatusbar: true,
            statusbarheight: 20,
            ready: function () {
                if (sessvars.state) {
                    var state = sessvars.state;
                    sessvars.$.clearMem();
                    $("#jqxgrid").jqxGrid('loadstate', state);
                } else {
                    loadDefault();
                }

                showCDFOpps("#jqxgrid");

                $.unblockUI();
            },
            columns: [
                { text: 'Opportunity ID', columntype: 'textbox', datafield: 'CRMOppID', width: '9%', cellsalign: 'left', renderer: columnsrenderer,
                    cellsrenderer: function (row) {
                        var formatedData;
                        var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', row);
                        formatedData = "<div style='top:20%;position:absolute;height:0.5px;margin-top:0.25px'><a style='text-align:left' href='#Edit'>" + dataRecord.CRMOppID + "</a></div>";
                        return '<div style="font-size: 12px;font-family: Verdana;text-align: left;">' + formatedData + '</div>'
                    }
                },
                { text: 'Opportunity Name', columntype: 'textbox', datafield: 'OppName', width: '13%', cellsalign: 'left', renderer: columnsrenderer,
                    cellsrenderer: function (row) {
                        var formatedData;
                        var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', row);
                        formatedData = "<div style='top:20%;position:absolute;height:0.5px;margin-top:0.25px'><a style='text-align:left;' href='#Edit'>" + dataRecord.OppName + "</a></div>";
                        return '<div style="font-size: 12px;font-family: Verdana;text-align: left;">' + formatedData + '</div>'
                    }
                },
                { text: 'Customer Name', columntype: 'textbox', datafield: 'CompanyName', width: '13%', cellsalign: 'left', renderer: columnsrenderer },
                { text: 'Stage', columntype: 'textbox', datafield: 'OppStatusName', width: '5%', cellsalign: 'left', renderer: columnsrenderer },
                { text: 'Probability', columntype: 'textbox', datafield: 'OppProbability', width: '7%', cellsalign: 'right', cellsformat: 'p', renderer: columnsrenderer },
                { text: 'Total Sell', columntype: 'textbox', datafield: 'QuotedAmount', width: '10%', cellsalign: 'right', cellsformat: 'c2', renderer: columnsrenderer,
                    //Create the sum of Total Quoted
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'Total Cost', columntype: 'textbox', datafield: 'QuotedCost', width: '10%', cellsalign: 'right', cellsformat: 'c2', renderer: columnsrenderer,
                    //Create the sum of Total Cost
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'Margin', columntype: 'textbox', datafield: 'QuotedMargin', width: '5%', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                { text: '# of Quotes', columntype: 'textbox', datafield: 'NumofQuotes', width: '7%', cellsalign: 'right', renderer: columnsrenderer, cellsrenderer: function (row) {
                    var formatedData;
                    var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', row);
                    formatedData = "<div style='top:20%;position:absolute;height:0.5px;margin-top:0.25px;left:40%'><a style='text-align:center' href='#" + row + "'>" + dataRecord.NumofQuotes + "</a></div>";
                    return '<div style="font-size: 12px;font-family: Verdana;text-align: center;">' + formatedData + '</div>'
                }
                },
                { text: 'Close Date', columntype: 'date', cellsformat: 'yyyy-MM-dd', datafield: 'OppCloseDate', width: '6%', renderer: columnsrenderer, filterable: false },
                { text: 'Create Date', columntype: 'date', filtertype: 'date', cellsformat: 'yyyy-MM-dd', datafield: 'CreateDT', width: '6%', renderer: columnsrenderer,
                    cellsrenderer: function (index, datafield, value, defaultvalue, column, rowdata) {
                        if (value != '') {
                            var localTime = moment.utc(value).toDate();
                            if (editrow == rowdata.OppID) {
                                localTime = value;
                                editrow = -1;
                            }
                            return "<div style='top:18%;position:absolute;height:0.5px;margin-top:0.25px;left:5%'>" + dataAdapter.formatDate(localTime, 'yyyy-MM-dd') + "</div>";
                        }
                        else {
                            return value;
                        }
                    }
                },
                { text: 'Owner Name', columntype: 'textbox', datafield: 'OppOwnerName', width: '9%', cellsalign: 'left', renderer: columnsrenderer },
                { text: 'OppID', datafield: 'OppID', hidden: true },
                { text: 'OppStatusId', datafield: 'OppStatusId', hidden: true },
                { text: 'StageType', datafield: 'StageType', hidden: true },
                { text: 'OppOwner', datafield: 'OppOwner', hidden: true },
                { text: 'ClientDefinedTotal1', datafield: 'ClientDefinedTotal1', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal2', datafield: 'ClientDefinedTotal2', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal3', datafield: 'ClientDefinedTotal3', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal4', datafield: 'ClientDefinedTotal4', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal5', datafield: 'ClientDefinedTotal5', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal6', datafield: 'ClientDefinedTotal6', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal7', datafield: 'ClientDefinedTotal7', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal8', datafield: 'ClientDefinedTotal8', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal9', datafield: 'ClientDefinedTotal9', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedTotal10', datafield: 'ClientDefinedTotal10', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer,
                    //Create the sum of ClientDefinedTotal1
                    aggregates: ['sum'],
                    aggregatesrenderer: function (aggregates, column, element, summaryData) {
                        var renderstring = "<div class='jqx-grid-cell jqx-grid-cell-arctic jqx-grid-cell-pinned jqx-grid-cell-pinned-arctic' style='float: left; width: 100%; height: 100%;'>";
                        $.each(aggregates, function (key, value) {
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: left; overflow: hidden;"></div>';
                            renderstring += '<div style="font-size:13px; position: relative; margin: 1px; text-align: right; overflow: hidden;">' + value + '</div>';
                        });
                        renderstring += "</div>";
                        return renderstring;
                    }
                },
                { text: 'ClientDefinedText1', datafield: 'ClientDefinedText1', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                { text: 'ClientDefinedText2', datafield: 'ClientDefinedText2', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                { text: 'ClientDefinedText3', datafield: 'ClientDefinedText3', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                { text: 'ClientDefinedText4', datafield: 'ClientDefinedText4', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                { text: 'ClientDefinedText5', datafield: 'ClientDefinedText5', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer }
            ]
        });

        if (sessvars.records) {
            var indice = sessvars.indice;
            $("#menu_top").jqxDropDownList('selectIndex', -1);
        }

        var manageOpportunitiesInCRM = false;
        var displayCRMData = 'V';
        $.ajax({
            url: '/Home/ManageOpportunitiesInCRM',
            type: 'GET',
            success: function (result) {
                manageOpportunitiesInCRM = result.ManageOppysInCRM;
                displayCRMData = result.CRMData;
            },
            error: function (result) {
                alert(result);
            }
        });

        // initialize the popup window and buttons.
        $("#popupWindow").jqxWindow({
            width: 470, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#Cancel"), modalOpacity: 0.3
        });

        $("#popupWindow").off('open').on('open', function () {
            $("#oppName").jqxInput('selectAll');
        });

        // initialize the popup window and buttons.
        $("#popupQuotes").jqxWindow({
            width: 980, maxWidth: 1000, height: 410, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#Cancel"), modalOpacity: 0.3
        });

        $("#popupReassignQuotes").jqxWindow({
            width: 950, height: 408, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#Cancel"), modalOpacity: 0.3
        });

        $("#popupUserViews").jqxWindow({
            width: 312, maxWidth: 400, height: 500, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#closeUVWndw"), modalOpacity: 0.3
        });

        // prepare the data
        var href = window.location.href.split('/');

        // initialize the input fields.
        $("#stageCDMenu").jqxMenu({ width: '210', height: '30px', autoOpen: false, autoCloseOnMouseLeave: false, autoCloseOnClick: false, showTopLevelArrows: true, theme: 'arctic' });
        $("#stageCDMenu").css('visibility', 'visible');
        $("#S_Open").jqxCheckBox({ height: '20px', checked: true, theme: 'arctic' });
        $("#S_Won").jqxCheckBox({ height: '20px', theme: 'arctic' });
        $("#S_Lost").jqxCheckBox({ height: '20px', theme: 'arctic' });
        $("#CD_ThisMonth").jqxCheckBox({ height: '20px', checked: true, theme: 'arctic' });
        $("#CD_LastMonth").jqxCheckBox({ height: '20px', theme: 'arctic' });
        $("#CD_NextMonth").jqxCheckBox({ height: '20px', theme: 'arctic' });
        $("#CD_ThisQuarter").jqxCheckBox({ height: '20px', theme: 'arctic' });
        $("#CD_LastQuarter").jqxCheckBox({ height: '20px', theme: 'arctic' });
        $("#CD_NextQuarter").jqxCheckBox({ height: '20px', theme: 'arctic' });

        $("#S_Apply").jqxButton({ theme: 'arctic' });
        $("#S_Cancel").jqxButton({ theme: 'arctic' });
        $("#S_Cancel").off('click').on('click', function () {
            $("#stageCDMenu").jqxMenu('closeItem', 'StageCloseDate');
        });

        $(document).off('keyup').on('keyup', function (evt) {
            if (evt.keyCode == 27) {
                $("#stageCDMenu").jqxMenu('closeItem', 'StageCloseDate');
            }
        });

        $("#oppId").jqxInput({ theme: 'arctic', disabled: true });
        $("#crmOppId").jqxInput({ theme: 'arctic' });
        $("#oppName").jqxInput({ theme: 'arctic' });
        $("#companyName").jqxInput({ theme: 'arctic' });
        $("#probability").jqxNumberInput({ width: '190px', height: '25px', min: 0, max: 100, digits: 3, inputMode: 'simple', decimalDigits: 0 });
        $("#closeDate").jqxDateTimeInput({ theme: 'arctic', width: '220px', height: '23px', formatString: 'MM/dd/yyyy' });
        $("#createDT").jqxDateTimeInput({ formatString: 'MM/dd/yyyy' });


        var sourceDdl =
                {
                    url: "/Admin/OppStatuses",
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
        });
        $("#oppStatus").jqxDropDownList({ source: dataAdapterDdl, displayMember: "StrMessage", valueMember: "MgUserId", theme: 'arctic', width: 220, height: 23 });

        var sourceOdl =
                {
                    url: "/Admin/UserList",
                    datatype: "json",
                    datafields:
                    [
                        { name: 'StrMessage', type: 'string' },
                        { name: 'MgUserId', type: 'int' }
                    ],
                    id: 'MgUserId'
                };

        var dataAdapterOdl = new $.jqx.dataAdapter(sourceOdl, {
            formatData: function (datas) {
                $.extend(datas, {
                    featureClass: "P",
                    style: "full",
                    maxRows: 50
                });
                return datas;
            }
        });
        $("#owner").jqxDropDownList({ source: dataAdapterOdl, displayMember: "StrMessage", valueMember: "MgUserId", theme: 'arctic', width: 220, height: 23 });

        $("#oppId").width(220);
        $("#oppId").height(23);
        $("#crmOppId").width(220);
        $("#crmOppId").height(23);
        $("#oppName").width(220);
        $("#oppName").height(23);
        $("#companyName").width(220);
        $("#companyName").height(23);

        var currChange = false;
        var currStatus = 0;

        var numberFormat = function (x, decimals, dec_point, thousands_sep) {
            dec_point = typeof dec_point !== 'undefined' ? dec_point : '.';
            thousands_sep = typeof thousands_sep !== 'undefined' ? thousands_sep : ',';

            var parts = x.toFixed(decimals).split('.');
            parts[0] = parts[0].replace(/\B(?=(\d{3})+(?!\d))/g, thousands_sep);

            return parts.join(dec_point);
        };

        getFloat = function (x, dec_point, thousands_sep) {
            dec_point = typeof dec_point !== 'undefined' ? dec_point : '.';
            thousands_sep = typeof thousands_sep !== 'undefined' ? thousands_sep : ',';

            var parts = x.split(dec_point);
            parts[0] = parts[0].replace(/,/g, "");

            return parseFloat(parts.join(dec_point));
        }

        var editOpportunity = function (row) {
            // open the popup window when the user clicks a button.
            editrow = row;
            var offset = $("#jqxgrid").offset();
            $("#popupWindow").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 30} });
            // get the clicked row's data and initialize the input fields.
            var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', editrow);
            $("#oppId").val(dataRecord.OppID);
            $("#crmOppId").val(dataRecord.CRMOppID);
            $("#oppName").val(dataRecord.OppName);
            $("#companyName").val(dataRecord.CompanyName);
            $("#closeDate").jqxDateTimeInput('setDate', dataRecord.OppCloseDate);

            currStatus = dataRecord.OppStatusId;
            $("#oppStatus").jqxDropDownList('val', dataRecord.OppStatusId);
            $("#probability").val(dataRecord.OppProbability);
            $("#owner").jqxDropDownList('val', dataRecord.OppOwner);

            if (dataRecord.QuotedAmount != null) {
                $("#quotedAmtInput").val(numberFormat(dataRecord.QuotedAmount, 2));
            }

            if (dataRecord.NumofQuotes > 0) {
                $("#quotedAmtInput").jqxInput({ disabled: true });
            }
            else {
                $("#quotedAmtInput").jqxInput({ disabled: false });
            }

            $("#quotedCost").jqxNumberInput({ decimal: dataRecord.QuotedCost });

            $("#quotedMargin").val(dataRecord.QuotedMargin);
            $("#numQuotes").val(dataRecord.NumofQuotes);
            $("#createDT").jqxDateTimeInput('setDate', dataRecord.CreateDT);

            if (!manageOpportunitiesInCRM) {
                $("#crmOppId").jqxInput({ disabled: true });
                //                $("#oppName").jqxInput({ disabled: false });
                //                $("#companyName").jqxInput({ disabled: false });

                //      $("#owner").jqxDropDownList({ disabled: false });
            }
            else {
                $("#crmOppId").jqxInput({ disabled: true });
                //                $("#oppName").jqxInput({ disabled: true });
                //                $("#companyName").jqxInput({ disabled: true });
                //                $("#quotedAmtInput").jqxInput({ disabled: true });

                //     $("#owner").jqxDropDownList({ disabled: true });
            }

            if (displayCRMData == 'E' || displayCRMData == 'V') {
                $("#TRCloseDate").css("display", "table-row");
                $("#TRProbability").css("display", "table-row");
                $("#TRStatus").css("display", "table-row");
            }
            else {
                $("#TRCloseDate").css("display", "none");
                $("#TRProbability").css("display", "none");
                $("#TRStatus").css("display", "none");
            }

            if (displayCRMData == 'V') {
                $("#closeDate").jqxDateTimeInput({ disabled: true });
                $("#oppStatus").jqxDropDownList({ disabled: true });
                $("#probability").jqxNumberInput({ disabled: true });
            }
            else {
                $("#closeDate").jqxDateTimeInput({ disabled: false });
                $("#oppStatus").jqxDropDownList({ disabled: false });
                $("#probability").jqxNumberInput({ disabled: false });
            }

            $.each(CDFDisplayed, function (idx, obj) {
                if (obj.Field == 'ClientDefinedTotal1') {
                    //                    $("#TRcdfTotal1").css("display", "table-row");
                    //                    $("#TxtcdfTotal1").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal1 !== 'undefined' && dataRecord.ClientDefinedTotal1 > 0) {
                        $("#cdfTotal1").val(numberFormat(dataRecord.ClientDefinedTotal1, 2));
                    }
                    else {
                        $("#cdfTotal1").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal2') {
                    //                    $("#TRcdfTotal2").css("display", "table-row");
                    //                    $("#TxtcdfTotal2").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal2 !== 'undefined' && dataRecord.ClientDefinedTotal2 > 0) {
                        $("#cdfTotal2").val(numberFormat(dataRecord.ClientDefinedTotal2, 2));
                    }
                    else {
                        $("#cdfTotal2").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal3') {
                    //                    $("#TRcdfTotal3").css("display", "table-row");
                    //                    $("#TxtcdfTotal3").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal3 !== 'undefined' && dataRecord.ClientDefinedTotal3 > 0) {
                        $("#cdfTotal3").val(numberFormat(dataRecord.ClientDefinedTotal3, 2));
                    }
                    else {
                        $("#cdfTotal3").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal4') {
                    //                    $("#TRcdfTotal4").css("display", "table-row");
                    //                    $("#TxtcdfTotal4").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal4 !== 'undefined' && dataRecord.ClientDefinedTotal4 > 0) {
                        $("#cdfTotal4").val(numberFormat(dataRecord.ClientDefinedTotal4, 2));
                    }
                    else {
                        $("#cdfTotal4").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal5') {
                    //                    $("#TRcdfTotal5").css("display", "table-row");
                    //                    $("#TxtcdfTotal5").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal5 !== 'undefined' && dataRecord.ClientDefinedTotal5 > 0) {
                        $("#cdfTotal5").val(numberFormat(dataRecord.ClientDefinedTotal5, 2));
                    }
                    else {
                        $("#cdfTotal5").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal6') {
                    //                    $("#TRcdfTotal6").css("display", "table-row");
                    //                    $("#TxtcdfTotal6").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal6 !== 'undefined' && dataRecord.ClientDefinedTotal6 > 0) {
                        $("#cdfTotal6").val(numberFormat(dataRecord.ClientDefinedTotal6, 2));
                    }
                    else {
                        $("#cdfTotal6").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal7') {
                    //                    $("#TRcdfTotal7").css("display", "table-row");
                    //                    $("#TxtcdfTotal7").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal7 !== 'undefined' && dataRecord.ClientDefinedTotal7 > 0) {
                        $("#cdfTotal7").val(numberFormat(dataRecord.ClientDefinedTotal7, 2));
                    }
                    else {
                        $("#cdfTotal7").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal8') {
                    //                    $("#TRcdfTotal8").css("display", "table-row");
                    //                    $("#TxtcdfTotal8").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal8 !== 'undefined' && dataRecord.ClientDefinedTotal8 > 0) {
                        $("#cdfTotal8").val(numberFormat(dataRecord.ClientDefinedTotal8, 2));
                    }
                    else {
                        $("#cdfTotal8").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal9') {
                    //                    $("#TRcdfTotal9").css("display", "table-row");
                    //                    $("#TxtcdfTotal9").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal9 !== 'undefined' && dataRecord.ClientDefinedTotal9 > 0) {
                        $("#cdfTotal9").val(numberFormat(dataRecord.ClientDefinedTotal9, 2));
                    }
                    else {
                        $("#cdfTotal9").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedTotal10') {
                    //                    $("#TRcdfTotal10").css("display", "table-row");
                    //                    $("#TxtcdfTotal10").html(obj.ColumnHeader);
                    if (dataRecord.ClientDefinedTotal10 !== 'undefined' && dataRecord.ClientDefinedTotal10 > 0) {
                        $("#cdfTotal10").val(numberFormat(dataRecord.ClientDefinedTotal10, 2));
                    }
                    else {
                        $("#cdfTotal10").val(0);
                    }
                }

                if (obj.Field == 'ClientDefinedText1') {
                    $("#TRcdfText1").css("display", "table-row");
                    $("#TxtcdfText1").html(obj.ColumnHeader);
                    $("#cdfText1").val(dataRecord.ClientDefinedText1);
                }

                if (obj.Field == 'ClientDefinedText2') {
                    $("#TRcdfText2").css("display", "table-row");
                    $("#TxtcdfText2").html(obj.ColumnHeader);
                    $("#cdfText2").val(dataRecord.ClientDefinedText2);
                }

                if (obj.Field == 'ClientDefinedText3') {
                    $("#TRcdfText3").css("display", "table-row");
                    $("#TxtcdfText3").html(obj.ColumnHeader);
                    $("#cdfText3").val(dataRecord.ClientDefinedText3);
                }

                if (obj.Field == 'ClientDefinedText4') {
                    $("#TRcdfText4").css("display", "table-row");
                    $("#TxtcdfText4").html(obj.ColumnHeader);
                    $("#cdfText4").val(dataRecord.ClientDefinedText4);
                }

                if (obj.Field == 'ClientDefinedText5') {
                    $("#TRcdfText5").css("display", "table-row");
                    $("#TxtcdfText5").html(obj.ColumnHeader);
                    $("#cdfText5").val(dataRecord.ClientDefinedText5);
                }
            });

            $("#oppPopupTitle").text('Edit');

            // show the popup window.
            $("#popupWindow").jqxWindow('open');
        };

        $("#addrowbutton").jqxButton({ width: '70px', height: '23px' });

        // create new row.
        $("#addrowbutton").off('click').on('click', function () {
            if (manageOpportunitiesInCRM) {
                alert('You are tied to a CRMSystem.  All Opportunities must be created in the CRMSystem.');
            }
            else {
                var offset = $("#jqxgrid").offset();
                $("#popupWindow").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 30} });
                // get the clicked row's data and initialize the input fields.
                $("#TRCloseDate").css("display", "table-row");
                $("#TRProbability").css("display", "table-row");
                $("#TRStatus").css("display", "table-row");

                $("#oppId").val('');
                $("#crmOppId").val('');
                $("#oppName").val('');
                $("#companyName").val('');
                $("#probability").val(0);

                $("#closeDate").jqxDateTimeInput('setDate', new Date());

                $("#crmOppId").attr('disabled', 'disabled');

                $("#oppStatus").jqxDropDownList('selectIndex', 0);

                $("#owner").jqxDropDownList('selectIndex', 0);

                $("#quotedAmtInput").val(0);
                $("#quotedAmtInput").removeAttr('disabled');
                $("#quotedCost").jqxNumberInput({ decimal: 0 });

                $("#quotedMargin").val(0);
                $("#numQuotes").val(0);
                $("#createDT").jqxDateTimeInput('setDate', new Date());

                $.each(CDFDisplayed, function (idx, obj) {
                    if (obj.Field == 'ClientDefinedTotal1') {
                        //                        $("#TRcdfTotal1").css("display", "table-row");
                        //                        $("#TxtcdfTotal1").html(obj.ColumnHeader);
                        $("#cdfTotal1").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal2') {
                        //                        $("#TRcdfTotal2").css("display", "table-row");
                        //                        $("#TxtcdfTotal2").html(obj.ColumnHeader);
                        $("#cdfTotal2").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal3') {
                        //                        $("#TRcdfTotal3").css("display", "table-row");
                        //                        $("#TxtcdfTotal3").html(obj.ColumnHeader);
                        $("#cdfTotal3").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal4') {
                        //                        $("#TRcdfTotal4").css("display", "table-row");
                        //                        $("#TxtcdfTotal4").html(obj.ColumnHeader);
                        $("#cdfTotal4").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal5') {
                        //                        $("#TRcdfTotal5").css("display", "table-row");
                        //                        $("#TxtcdfTotal5").html(obj.ColumnHeader);
                        $("#cdfTotal5").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal6') {
                        //                        $("#TRcdfTotal6").css("display", "table-row");
                        //                        $("#TxtcdfTotal6").html(obj.ColumnHeader);
                        $("#cdfTotal6").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal7') {
                        //                        $("#TRcdfTotal7").css("display", "table-row");
                        //                        $("#TxtcdfTotal7").html(obj.ColumnHeader);
                        $("#cdfTotal7").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal8') {
                        //                        $("#TRcdfTotal8").css("display", "table-row");
                        //                        $("#TxtcdfTotal8").html(obj.ColumnHeader);
                        $("#cdfTotal8").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal9') {
                        //                        $("#TRcdfTotal9").css("display", "table-row");
                        //                        $("#TxtcdfTotal9").html(obj.ColumnHeader);
                        $("#cdfTotal9").val(0);
                    }

                    if (obj.Field == 'ClientDefinedTotal10') {
                        //                        $("#TRcdfTotal10").css("display", "table-row");
                        //                        $("#TxtcdfTotal10").html(obj.ColumnHeader);
                        $("#cdfTotal10").val(0);
                    }

                    if (obj.Field == 'ClientDefinedText1') {
                        $("#TRcdfText1").css("display", "table-row");
                        $("#TxtcdfText1").html(obj.ColumnHeader);
                        $("#cdfText1").val('');
                    }

                    if (obj.Field == 'ClientDefinedText2') {
                        $("#TRcdfText2").css("display", "table-row");
                        $("#TxtcdfText2").html(obj.ColumnHeader);
                        $("#cdfText2").val('');
                    }

                    if (obj.Field == 'ClientDefinedText3') {
                        $("#TRcdfText3").css("display", "table-row");
                        $("#TxtcdfText3").html(obj.ColumnHeader);
                        $("#cdfText3").val('');
                    }

                    if (obj.Field == 'ClientDefinedText4') {
                        $("#TRcdfText4").css("display", "table-row");
                        $("#TxtcdfText4").html(obj.ColumnHeader);
                        $("#cdfText4").val('');
                    }

                    if (obj.Field == 'ClientDefinedText5') {
                        $("#TRcdfText5").css("display", "table-row");
                        $("#TxtcdfText5").html(obj.ColumnHeader);
                        $("#cdfText5").val('');
                    }
                });

                $("#oppPopupTitle").text('Add');

                // show the popup window.
                $("#popupWindow").jqxWindow('open');
            }
        });

        $("#editrowbutton").jqxButton({ width: '70px', height: '23px' });
        // create new row.
        $("#editrowbutton").off('click').on('click', function () {
            // open the popup window when the user clicks a button.
            var row = $("#jqxgrid").jqxGrid('getselectedrowindex');
            editOpportunity(row);
        });

        $("#delrowbutton").jqxButton({ width: '70px', height: '23px' });
        // create new row.
        $("#delrowbutton").off('click').on('click', function () {
            // open the popup window when the user clicks a button.
            var row = $("#jqxgrid").jqxGrid('getselectedrowindex');
            if (row > -1) {
                var rowscount = $("#jqxgrid").jqxGrid('getdatainformation').rowscount;
                var id = $("#jqxgrid").jqxGrid('getrowid', row);
                var datarow = $("#jqxgrid").jqxGrid('getrowdata', row);

                $.ajax({
                    url: '/Home/QuoteDataGrid',
                    data: { opportunityId: datarow.OppID },
                    type: 'GET',
                    success: function (result) {
                        if (result.length == 0) {
                            if (row >= 0) {
                                var delOpp = confirm("Are you sure you want to delete this?");
                                if (delOpp) {
                                    $.ajax({
                                        url: '/Home/DeleteOpportunityRow',
                                        data: { opportunityId: datarow.OppID },
                                        type: 'DELETE',
                                        success: function (result) {
                                            if (result.MgUserId > 0) {
                                                var commit = $("#jqxgrid").jqxGrid('deleterow', id);

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
                        }
                        else {
                            alert("You can't delete the opportunity because has quotes.");
                        }
                    },
                    error: function (result) {
                        alert(result);
                    }
                });
            }
        });

        var reloadGrid = function () {
            $("#jqxgrid").jqxGrid('clearfilters');
            $("#jqxgrid").jqxGrid('clear');
            loadArchived();
            $("#jqxgrid").jqxGrid('autoresizecolumns');
        };

        var sourceVw =
                {
                    url: "/Home/UserViews",
                    datatype: "json",
                    datafields:
                    [
                        { name: 'StrMessage', type: 'string' },
                        { name: 'UserViewId', type: 'string' }
                    ],
                    id: 'UserViewId'
                };

        var dataAdapterVw = new $.jqx.dataAdapter(sourceVw, {
            formatData: function (datas) {
                $.extend(datas, {
                    featureClass: "P",
                    style: "full",
                    maxRows: 50
                });
                return datas;
            }
        });

        dataAdapterVw.dataBind();
        // get the menu items. The first parameter is the item's id. The second parameter is the parent item's id. The 'items' parameter represents 
        // the sub items collection name. Each jqxTree item has a 'label' property, but in the JSON data, we have a 'text' field. The last parameter 
        // specifies the mapping between the 'text' and 'label' fields.  
        //var recordsVw = dataAdapterVw.getRecordsHierarchy('id', 'parentid', 'items', [{ name: 'text', map: 'label'}]);

        var refreshGrid = function () {
            var state = $("#jqxgrid").jqxGrid('savestate');

            var title = "Showing ";
            var arr = strCustomFilter.substring(0, strCustomFilter.length - 1).split('=');

            var filterList = arr[1].split(';');

            var filterdata0 = filterList[0].split('/');
            stageArray = new Array();
            $('#S_Open').jqxCheckBox({ checked: false });
            $('#S_Won').jqxCheckBox({ checked: false });
            $('#S_Lost').jqxCheckBox({ checked: false });

            stages = '';

            for (var m = 0; m < filterdata0.length; m++) {
                stageArray.push(filterdata0[m]);
                var lastChar = title.slice(-1);
                if (lastChar == "n" || lastChar == "t") {
                    title = title + "/";
                    stages = stages + "/";
                }
                if (filterdata0[m] == "Open") {
                    title = title + "Open";
                    stages = stages + "Open";
                    $('#S_Open').jqxCheckBox({ checked: true });
                }
                if (filterdata0[m] == "Won") {
                    title = title + "Won";
                    stages = stages + "Won";
                    $('#S_Won').jqxCheckBox({ checked: true });
                }
                if (filterdata0[m] == "Lost") {
                    title = title + "Lost";
                    stages = stages + "Lost";
                    $('#S_Lost').jqxCheckBox({ checked: true });
                }
            }

            //filterStageType();

            title = title + " opportunities closing ";

            var filterdata = filterList[1].split('/');
            var initialDateT = new Date();
            var finalDateT = new Date();
            initialDate = new Date(1900, 01, 01);
            finalDate = new Date(1900, 01, 01);
            var tFirstDay = new Date(1900, 01, 01);
            var tLastDay = new Date(1900, 01, 01);

            $('#CD_ThisMonth').jqxCheckBox({ checked: false });
            $('#CD_LastMonth').jqxCheckBox({ checked: false });
            $('#CD_NextMonth').jqxCheckBox({ checked: false });
            $('#CD_ThisQuarter').jqxCheckBox({ checked: false });
            $('#CD_LastQuarter').jqxCheckBox({ checked: false });
            $('#CD_NextQuarter').jqxCheckBox({ checked: false });
            $('#CD_DateRange').jqxCheckBox({ checked: false });

            for (var j = 0; j < filterdata.length; j++) {
                var lastChar = title.slice(-1);
                if (lastChar == "h" || lastChar == "r") {
                    title = title + "/";
                }
                if (filterdata[j] == "THIS_MONTH") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    initialDateT = new Date(y, m, 1);
                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }

                    if (m < 11) {
                        finalDateT = new Date(y, m + 1, 0);
                    }
                    if (m == 11) {
                        y = y + 1, m = 0;
                        finalDateT = new Date(y, m, 0);
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "This Month";
                    $('#CD_ThisMonth').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "LAST_MONTH") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    finalDateT = new Date(y, m, 0);
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }

                    if (m == 0) {
                        y = y - 1, m = 11;
                        initialDateT = new Date(y, m, 1);
                    }
                    else {
                        initialDateT = new Date(y, m - 1, 1);
                    }
                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    title = title + "Last Month";
                    $('#CD_LastMonth').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "NEXT_MONTH") {
                    var ly = y = dateJ.getFullYear(), lm = m = dateJ.getMonth();
                    if (m < 11) {
                        initialDateT = new Date(y, m + 1, 1);
                    }
                    else {
                        y = y + 1, m = 1;
                        initialDateT = new Date(y, m, 1);
                    }
                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }

                    if (lm < 10) {
                        finalDateT = new Date(ly, lm + 2, 0);
                    }
                    if (lm == 10) {
                        ly = ly + 1, lm = 0;
                        finalDateT = new Date(ly, lm, 0);
                    }
                    if (lm == 11) {
                        ly = ly + 1, lm = 1;
                        finalDateT = new Date(ly, lm, 0);
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "Next Month";
                    $('#CD_NextMonth').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "THIS_QUARTER") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    if (m < 3) {
                        initialDateT = InitFQ;
                        finalDateT = FinFQ;
                    }
                    if (m >= 3 && m < 6) {
                        initialDateT = InitSQ;
                        finalDateT = FinSQ;
                    }
                    if (m >= 6 && m < 9) {
                        initialDateT = InitTQ;
                        finalDateT = FinTQ;
                    }
                    if (m >= 9) {
                        initialDateT = InitFFQ;
                        finalDateT = FinFFQ;
                    }

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "This Quarter";
                    $('#CD_ThisQuarter').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "LAST_QUARTER") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    if (m < 3) {
                        var yyQ = y - 1;
                        InitFFQ = new Date(yyQ, 9, 1);
                        FinFFQ = new Date(y, 0, 0);
                        initialDateT = InitFFQ;
                        finalDateT = FinFFQ;
                    }
                    if (m >= 3 && m < 6) {
                        initialDateT = InitFQ;
                        finalDateT = FinFQ;
                    }
                    if (m >= 6 && m < 9) {
                        initialDateT = InitSQ;
                        finalDateT = FinSQ;
                    }
                    if (m >= 9) {
                        initialDateT = InitTQ;
                        finalDateT = FinTQ;
                    }

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "Last Quarter";
                    $('#CD_LastQuarter').jqxCheckBox({ checked: true });
                }
                if (filterdata[j] == "NEXT_QUARTER") {
                    var y = dateJ.getFullYear(), m = dateJ.getMonth();
                    if (m < 3) {
                        initialDateT = InitSQ;
                        finalDateT = FinSQ;
                    }
                    if (m >= 3 && m < 6) {
                        initialDateT = InitTQ;
                        finalDateT = FinTQ;
                    }
                    if (m >= 6 && m < 9) {
                        initialDateT = InitFFQ;
                        finalDateT = FinFFQ;
                    }
                    if (m >= 9) {
                        var yyQ = y + 1;
                        InitFQ = new Date(yyQ, 0, 1);
                        FinFQ = new Date(yyQ, 3, 0);
                        initialDateT = InitFQ;
                        finalDateT = FinFQ;
                    }

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }
                    title = title + "Next Quarter";
                    $('#CD_NextQuarter').jqxCheckBox({ checked: true });
                }
                if (filterdata[j].substring(0, 4) == "DATE") {
                    var dranges = filterdata[j].split('|');
                    initialDateT = new Date(dranges[1]);
                    finalDateT = new Date(dranges[2]);

                    if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                        initialDate = initialDateT;
                    }
                    if (finalDateT > finalDate) {
                        finalDate = finalDateT;
                    }

                    title = title + "Date Range";
                    $('#CD_DateRange').jqxCheckBox({ checked: true });
                    $("#drInitialDate").jqxDateTimeInput('setDate', initialDateT);
                    $("#drFinalDate").jqxDateTimeInput('setDate', finalDateT);
                }
            }

            $("#dateTitle").text(title + " - \'");
            $("#viewTitle").text(sessvars.viewTitle);

            reloadGrid();

            $("#jqxgrid").jqxGrid('loadstate', state);
        };

        var clearGrid = function () {
            $("#jqxgrid").jqxGrid('clearselection');
            $('#jqxgrid').jqxGrid('cleargroups');
            $("#jqxgrid").jqxGrid('removesort');
            $("#jqxgrid").jqxGrid('clearfilters');
        };

        //$("#menu_top").jqxMenu({ source: recordsVw, width: '78px', height: '30px', autoOpen: false, showTopLevelArrows: true });
        $("#menu_top").jqxDropDownList({ placeHolder: '<div style="text-align: center;">Views</div>', theme: 'arctic', source: dataAdapterVw, displayMember: "StrMessage", valueMember: "UserViewId", width: '210px', height: '30px', enableBrowserBoundsDetection: true, autoDropDownHeight: true });
        $("#menu_top").css('visibility', 'visible');
        $("#menu_top").jqxDropDownList('selectIndex', -1);


        var dateMenuFilters = [
                { value: 'THIS_MONTH', label: 'This Month' },
                { value: 'THIS_QUARTER', label: 'This Quarter' },
                { value: 'LAST_MONTH', label: 'Last Month' },
                { value: 'LAST_QUARTER', label: 'Last Quarter' },
                { value: 'NEXT_MONTH', label: 'Next Month' },
                { value: 'NEXT_QUARTER', label: 'Next Quarter' },
                { value: 'DTRANGE', label: 'Date Range' }
            ];
        var dateMenuFilterSource =
            {
                datatype: "array",
                datafields: [
                     { name: 'label', type: 'string' },
                     { name: 'value', type: 'string' }
                 ],
                localdata: dateMenuFilters
            };
        var dateMenuFilterAdapter = new $.jqx.dataAdapter(dateMenuFilterSource, {
            autoBind: true
        });

        function arrayUnique(array) {
            var a = array.concat();
            for (var i = 0; i < a.length; ++i) {
                for (var j = i + 1; j < a.length; ++j) {
                    if (a[i].OppID === a[j].OppID)
                        a.splice(j--, 1);
                }
            }

            return a;
        }

        var loadArchived = function () {
            $.ajax
                    (
                        {
                            type: 'GET',
                            url: '/Home/OpportunityDataByDateRange?t=' + Math.random(),
                            data: { initialDate: initialDate.toISOString(), finalDate: finalDate.toISOString(), stages: stages },
                            async: false,
                            datatype: "application/json",
                            success: function (msg) {
                                var a = $("#jqxgrid").jqxGrid('getboundrows');

                                for (var i = 0; i < a.length; ++i) {
                                    for (var j = 0; j < msg.length; ++j) {
                                        if (msg[j].OppCloseDate !== null && msg[j].OppCloseDate !== '') {
                                            msg[j].OppCloseDate = new Date(msg[j].OppCloseDate);
                                        }
                                        if (msg[j].CreateDT !== null && msg[j].CreateDate !== '') {
                                            msg[j].CreateDT = new Date(msg[j].CreateDT);
                                        }
                                        if (msg[j].OppID === a[i].OppID) {
                                            msg.splice(j--, 1);
                                            break;
                                        }
                                    }
                                }

                                $("#jqxgrid").jqxGrid('addrow', null, msg);

                                // $("#jqxgrid").jqxGrid({ source: dataAdapter });

                                $.unblockUI();
                            },
                            error: function (result) {
                                alert(result);
                            }
                        }
                );

            first = false;

            filterCloseDate();
            filterStageType();

            //save new state
            var getState = $("#jqxgrid").jqxGrid('savestate');
            var stateToSave = JSON.stringify(getState);
        };


        var yQ = dateJ.getFullYear();
        var yQQ = yQ + 1;
        var InitFQ = new Date(yQ, 0, 1);
        var FinFQ = new Date(yQ, 3, 0);
        var InitSQ = new Date(yQ, 3, 1);
        var FinSQ = new Date(yQ, 6, 0);
        var InitTQ = new Date(yQ, 6, 1);
        var FinTQ = new Date(yQ, 9, 0);
        var InitFFQ = new Date(yQ, 9, 1);
        var FinFFQ = new Date(yQQ, 0, 0);

        $("#S_Apply").off('click').on('click', function (event) {
            var title = "Showing ";
            stages = '';
            stageArray = new Array();
            dateArray = new Array();
            var initialDateT = new Date();
            var finalDateT = new Date();
            initialDate = new Date(1900, 01, 01);
            finalDate = new Date(1900, 01, 01);
            var tmpStages = '';
            combinations = '';
            strCustomFilter = "Custom=";

            if ($('#S_Open').jqxCheckBox('checked')) {
                title = title + "Open";
                stages = stages + "Open";
                tmpStages = tmpStages + "Open";
                stageArray.push("Open");
            }
            if ($('#S_Won').jqxCheckBox('checked')) {
                if (title.slice(-1) == "n") {
                    title = title + "/";
                    stages = stages + "/";
                    tmpStages = tmpStages + ",";
                }
                tmpStages = tmpStages + "Won";
                title = title + "Won";
                stages = stages + "Won";
                stageArray.push("Won");
            }
            if ($('#S_Lost').jqxCheckBox('checked')) {
                if (title.slice(-1) == "n") {
                    title = title + "/";
                    stages = stages + "/";
                    tmpStages = tmpStages + ",";
                }
                tmpStages = tmpStages + "Lost";
                title = title + "Lost";
                stages = stages + "Lost";
                stageArray.push("Lost");
            }

            strCustomFilter = strCustomFilter + stages + ";";

            title = title + " opportunities closing ";

            var selDates = '';

            if ($('#CD_ThisMonth').jqxCheckBox('checked')) {
                var y = dateJ.getFullYear(), m = dateJ.getMonth();
                initialDateT = new Date(y, m, 1);
                if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                    initialDate = initialDateT;
                }

                if (m < 11) {
                    finalDateT = new Date(y, m + 1, 0);
                }
                if (m == 11) {
                    y = y + 1, m = 0;
                    finalDateT = new Date(y, m, 0);
                }
                if (finalDateT > finalDate) {
                    finalDate = finalDateT;
                }

                title = title + "This Month";
                dateArray.push("THIS_MONTH");
                selDates = "THIS_MONTH";
                combinations = 'THIS_MONTH|' + tmpStages + '/';
            }
            if ($('#CD_LastMonth').jqxCheckBox('checked')) {
                var y = dateJ.getFullYear(), m = dateJ.getMonth();
                finalDateT = new Date(y, m, 0);
                if (finalDateT > finalDate) {
                    finalDate = finalDateT;
                }

                if (m == 0) {
                    y = y - 1, m = 11;
                    initialDateT = new Date(y, m, 1);
                }
                else {
                    initialDateT = new Date(y, m - 1, 1);
                }
                if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                    initialDate = initialDateT;
                }

                if (title.slice(-1) == "h") {
                    title = title + "/";
                    selDates = selDates + "/";
                }
                title = title + "Last Month";
                dateArray.push("LAST_MONTH");
                selDates = selDates + "LAST_MONTH";
                combinations = combinations + 'LAST_MONTH|' + tmpStages + '/';
            }
            if ($('#CD_NextMonth').jqxCheckBox('checked')) {
                var ly = y = dateJ.getFullYear(), lm = m = dateJ.getMonth();
                if (m < 11) {
                    initialDateT = new Date(y, m + 1, 1);
                }
                else {
                    y = y + 1, m = 1;
                    initialDateT = new Date(y, m, 1);
                }
                if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                    initialDate = initialDateT;
                }

                if (lm < 10) {
                    finalDateT = new Date(ly, lm + 2, 0);
                }
                if (lm == 10) {
                    ly = ly + 1, lm = 0;
                    finalDateT = new Date(ly, lm, 0);
                }
                if (lm == 11) {
                    ly = ly + 1, lm = 1;
                    finalDateT = new Date(ly, lm, 0);
                }
                if (finalDateT > finalDate) {
                    finalDate = finalDateT;
                }

                if (title.slice(-1) == "h") {
                    title = title + "/";
                    selDates = selDates + "/";
                }
                title = title + "Next Month";
                dateArray.push("NEXT_MONTH");
                selDates = selDates + "NEXT_MONTH";
                combinations = combinations + 'NEXT_MONTH|' + tmpStages + '/';
            }
            if ($('#CD_ThisQuarter').jqxCheckBox('checked')) {
                var y = dateJ.getFullYear(), m = dateJ.getMonth();
                if (m < 3) {
                    initialDateT = InitFQ;
                    finalDateT = FinFQ;
                }
                if (m >= 3 && m < 6) {
                    initialDateT = InitSQ;
                    finalDateT = FinSQ;
                }
                if (m >= 6 && m < 9) {
                    initialDateT = InitTQ;
                    finalDateT = FinTQ;
                }
                if (m >= 9) {
                    initialDateT = InitFFQ;
                    finalDateT = FinFFQ;
                }

                if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                    initialDate = initialDateT;
                }
                if (finalDateT > finalDate) {
                    finalDate = finalDateT;
                }

                var lastChar = title.slice(-1);
                if (lastChar == "h") {
                    title = title + "/";
                    selDates = selDates + "/";
                }
                title = title + "This Quarter";
                dateArray.push("THIS_QUARTER");
                selDates = selDates + "THIS_QUARTER";
                combinations = combinations + 'THIS_QUARTER|' + tmpStages + '/';
            }
            if ($('#CD_LastQuarter').jqxCheckBox('checked')) {
                var y = dateJ.getFullYear(), m = dateJ.getMonth();
                if (m < 3) {
                    var yyQ = y - 1;
                    InitFFQ = new Date(yyQ, 9, 1);
                    FinFFQ = new Date(y, 0, 0);
                    initialDateT = InitFFQ;
                    finalDateT = FinFFQ;
                }
                if (m >= 3 && m < 6) {
                    initialDateT = InitFQ;
                    finalDateT = FinFQ;
                }
                if (m >= 6 && m < 9) {
                    initialDateT = InitSQ;
                    finalDateT = FinSQ;
                }
                if (m >= 9) {
                    initialDateT = InitTQ;
                    finalDateT = FinTQ;
                }

                if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                    initialDate = initialDateT;
                }
                if (finalDateT > finalDate) {
                    finalDate = finalDateT;
                }

                var lastChar = title.slice(-1);
                if (lastChar == "h" || lastChar == "r") {
                    title = title + "/";
                    selDates = selDates + "/";
                }
                title = title + "Last Quarter";
                dateArray.push("LAST_QUARTER");
                selDates = selDates + "LAST_QUARTER";
                combinations = combinations + 'LAST_QUARTER|' + tmpStages + '/';
            }
            if ($('#CD_NextQuarter').jqxCheckBox('checked')) {
                var y = dateJ.getFullYear(), m = dateJ.getMonth();
                if (m < 3) {
                    initialDateT = InitSQ;
                    finalDateT = FinSQ;
                }
                if (m >= 3 && m < 6) {
                    initialDateT = InitTQ;
                    finalDateT = FinTQ;
                }
                if (m >= 6 && m < 9) {
                    initialDateT = InitFFQ;
                    finalDateT = FinFFQ;
                }
                if (m >= 9) {
                    var yyQ = y + 1;
                    InitFQ = new Date(yyQ, 0, 1);
                    FinFQ = new Date(yyQ, 3, 0);
                    initialDateT = InitFQ;
                    finalDateT = FinFQ;
                }

                if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                    initialDate = initialDateT;
                }
                if (finalDateT > finalDate) {
                    finalDate = finalDateT;
                }

                var lastChar = title.slice(-1);
                if (lastChar == "h" || lastChar == "r") {
                    title = title + "/";
                    selDates = selDates + "/";
                }
                title = title + "Next Quarter";
                dateArray.push("NEXT_QUARTER");
                selDates = selDates + "NEXT_QUARTER";
                combinations = combinations + 'NEXT_QUARTER|' + tmpStages + '/';
            }
            if ($('#CD_DateRange').jqxCheckBox('checked')) {
                var dateR = $("#drInitialDate").jqxDateTimeInput('getDate');
                if (dateR != null) {
                    initialDateT = dateR;
                }
                var dateRS = $("#drFinalDate").jqxDateTimeInput('getDate');
                if (dateRS != null) {
                    finalDateT = dateRS;
                }

                if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                    initialDate = initialDateT;
                }
                if (finalDateT > finalDate) {
                    finalDate = finalDateT;
                }

                var lastChar = title.slice(-1);
                if (lastChar == "h" || lastChar == "r") {
                    title = title + "/";
                    selDates = selDates + "/";
                }
                title = title + "Date Range";
                dateArray.push("DATE_RANGE");
                selDates = selDates + "DATE_RANGE|" + initialDate.toISOString() + "|" + finalDate.toISOString();
                // combinations = combinations + 'DATE_RANGE;' initialDate.toISOString() +';'+ finalDate.toISOString() + '|' + tmpStages + '/';
            }

            strCustomFilter = strCustomFilter + selDates + ";&";

            $("#dateTitle").text(title + " - \'");

            $("#stageCDMenu").jqxMenu('closeItem', 'StageCloseDate');

            loadArchived();

        });

        var standard = function () {
            stages = 'Open';
            stageArray.push("Open");

            var initialDateT = new Date();
            var finalDateT = new Date();
            initialDate = new Date(1900, 01, 01);
            finalDate = new Date(1900, 01, 01);

            var y = dateJ.getFullYear(), m = dateJ.getMonth();
            initialDateT = new Date(y, m, 1);
            if ((initialDateT < initialDate) || (initialDate.getFullYear() == 1900)) {
                initialDate = initialDateT;
            }

            if (m < 11) {
                finalDateT = new Date(y, m + 1, 0);
            }
            if (m == 11) {
                y = y + 1, m = 0;
                finalDateT = new Date(y, m, 0);
            }
            if (finalDateT > finalDate) {
                finalDate = finalDateT;
            }

            $('#S_Open').jqxCheckBox({ checked: true });
            $('#S_Won').jqxCheckBox({ checked: false });
            $('#S_Lost').jqxCheckBox({ checked: false });
            $('#CD_ThisMonth').jqxCheckBox({ checked: true });
            $('#CD_LastMonth').jqxCheckBox({ checked: false });
            $('#CD_NextMonth').jqxCheckBox({ checked: false });
            $('#CD_ThisQuarter').jqxCheckBox({ checked: false });
            $('#CD_LastQuarter').jqxCheckBox({ checked: false });
            $('#CD_NextQuarter').jqxCheckBox({ checked: false });
            $('#CD_DateRange').jqxCheckBox({ checked: false });

            strCustomFilter = "Custom=Open;THIS_MONTH;&";

            $("#dateTitle").text('Showing Open opportunities closing This Month - \'');
            $("#viewTitle").text('Standard\' view');
            $("#menu_top").jqxDropDownList('selectIndex', -1);

            loadArchived();

        };

        $("#menu_top").on('select', function (event) {
            if (event.args && (refresh == 0)) {
                var item = event.args.item;
                var selIndex = $('#menu_top').jqxDropDownList('selectedIndex');

                //if (item != "Views") {
                if (item) {
                    switch (item.value) {
                        case "SaveNewViewAs":
                            saveNewView();
                            break;
                        case "SaveAsDefault":
                            SaveAsName = '';
                            saveDefault();
                            break;
                        case "DeleteViews":
                            openUserViewDialog();
                            $("#menu_top").jqxDropDownList('selectIndex', -1);
                            break;
                        case "Standard":
                            clearGrid();
                            //standard();
                            break;
                        case "Archived":
                            clearGrid();
                            archived();
                            break;
                        case "StandardArchived":
                            SaveAsName = '';
                            $("#viewTitle").text('Standard\' view');
                            clearGrid();
                            standard();
                            break;
                        default:
                            SaveAsName = '';
                            if (item.label != 'Default') {
                                SaveAsName = item.label;
                            }
                            $("#viewTitle").text(item.label + '\' view');
                            clearGrid();
                            loadView(item.value);
                    }
                }
            }
        });

        var refresh = 0;

        $("#menu_top").on('bindingComplete', function (event) {
            if (refresh > 0) {
                //var item = $("#menu_top").jqxDropDownList('getItemByValue', refresh);
                $("#menu_top").jqxDropDownList('selectIndex', -1);
                refresh = 0;
            }
        });

        var getOpps = function (arch) {
            var filtergroup = new $.jqx.filter();
            var filtervalue = arch;
            var filtercondition = 'EQUAL';
            var filter1 = filtergroup.createfilter('stringfilter', filtervalue, filtercondition);
            var filter_or_operator = 1;
            filtergroup.addfilter(filter_or_operator, filter1);
            $("#jqxgrid").jqxGrid('addfilter', 'StageType', filtergroup);
            $("#jqxgrid").jqxGrid('applyfilters');
        };



        var archived = function () {
            getOpps("Open");
        };

        var saveDefault = function () {
            var getState = $("#jqxgrid").jqxGrid('savestate');
            var stateToSave = JSON.stringify(getState);
            stateToSave = strCustomFilter + stateToSave;
            // YOU ARE SAVING A STRING
            $.ajax({
                type: 'POST',
                url: '/Home/SaveDefaultUserView',
                data: { viewPref: stateToSave },
                success: function (response) {
                    refresh = response;
                    $("#viewTitle").text('Default\' view');
                    dataAdapterVw.dataBind();
                    var isIE = /*@cc_on!@*/false || !!document.documentMode;
                    if (isIE) {
                        alert("Changes to the Default view will take effect when Internet Explorer is restarted");
                    }
                }
            });
        };

        var loadView = function (loadViewId) {
            $.ajax({
                type: 'GET',
                url: '/Home/LoadViewById?t=' + Math.random(),
                data: { viewId: loadViewId },
                success: function (response) {
                    if (response != "") {
                        var arr = response.split('&');
                        if (arr.length > 1) {
                            response = JSON.parse(arr[1]);
                        }
                        else {
                            response = JSON.parse(response);
                        }

                        // IF YOU alert(response) YOU SHOULD GET AN [object Object] AND NOT A STRING. IF YOU GET A STRING SUCH HAS "{"width":"100%","height":400..." IT IS WRONG.														
                        $("#jqxgrid").jqxGrid('loadstate', response);

                        showCDFOpps("#jqxgrid");

                        strCustomFilter = arr[0] + '&';

                        if (arr.length > 1) {
                            rebuildFilter(arr[0], loadViewId);
                        }

                        $("#menu_top").jqxDropDownList('selectIndex', -1);
                    }
                },
                error: function (result) {
                    alert(result);
                }
            });
        };

        // initialize the popup window and buttons.
        $("#saveNewWdw").jqxWindow({
            width: 460, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#CancelViewName"), modalOpacity: 0.3
        });

        //        $("#dateRangeFilter").jqxWindow({
        //            width: 460, resizable: false, theme: 'arctic', isModal: true, autoOpen: false, cancelButton: $("#CancelFilter"), modalOpacity: 0.3
        //        });

        $("#viewName").jqxInput({ theme: 'arctic' });

        $("#saveNewWdw").off('open').on('open', function () {
            $("#viewName").jqxInput('selectAll');
        });

        var saveNewView = function () {
            var offset = $("#jqxgrid").offset();
            $("#saveNewWdw").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 30} });
            // get the clicked row's data and initialize the input fields.
            $("#viewName").val(SaveAsName);

            // show the popup window.
            $("#saveNewWdw").jqxWindow('open');
        };


        $("#CancelViewName").jqxButton({ theme: 'arctic' });
        $("#CancelViewName").off('click').on('click', function () {
            $("#saveNewWdw").jqxWindow('hide');
            $("#menu_top").jqxDropDownList('selectIndex', -1);
        });

        $("#SaveViewName").jqxButton({ theme: 'arctic' });
        // update the edited row when the user clicks the 'Save' button.
        $("#SaveViewName").off('click').on('click', function () {
            $("#SaveViewName").jqxButton({ disabled: true });
            $("#CancelViewName").jqxButton({ disabled: true });

            var existsView = -1;
            for (var m = 0; m < dataAdapterVw.records.length; m++) {
                if (dataAdapterVw.records[m].StrMessage.toUpperCase() == $("#viewName").val().toUpperCase()) {
                    existsView = m;
                }
            }

            var overwrite = true;

            if (existsView >= 0) {
                if (confirm("This view name already exists. Do you want to overwrite it?")) {
                    overwrite = true;
                }
                else {
                    overwrite = false;
                }
            }

            if (overwrite) {
                var getState = $("#jqxgrid").jqxGrid('savestate');
                var stateToSave = JSON.stringify(getState);

                stateToSave = strCustomFilter + stateToSave;

                var row = { viewName: $("#viewName").val(), viewPref: stateToSave };

                $.ajax({
                    url: '/Home/SaveNewUserView',
                    data: row,
                    datatype: "application/json",
                    type: 'POST',
                    success: function (result) {
                        if (result.MgUserId > 0) {
                            $("#saveNewWdw").jqxWindow('hide');
                            refresh = result.MgUserId;
                            $("#viewTitle").text(row.viewName + '\' view');
                            dataAdapterVw.dataBind();
                        } else {
                            alert(result.StrMessage);
                            $("#menu_top").jqxDropDownList('selectIndex', -1);
                        }

                        $("#SaveViewName").jqxButton({ disabled: false });
                        $("#CancelViewName").jqxButton({ disabled: false });
                    },
                    error: function (XMLHttpRequest, textStatus, errorThrown) {
                        alert("Error:" + XMLHttpRequest);
                        $("#menu_top").jqxDropDownList('selectIndex', -1);
                        $("#SaveViewName").jqxButton({ disabled: false });
                        $("#CancelViewName").jqxButton({ disabled: false });
                    }
                });
            }
            else {
                $("#SaveViewName").jqxButton({ disabled: false });
                $("#CancelViewName").jqxButton({ disabled: false });
            }
        });

        // Initialize jqxgridQuotes
        $("#jqxgridUserViews").jqxGrid({
            width: '100%',
            height: '400px',
            editable: true,
            theme: 'arctic',
            sortable: true,
            altrows: true,
            selectionmode: 'singlecell',
            columnsresize: true,
            columnsreorder: true,
            columns:
                [
                    { text: '', datafield: 'available', columntype: 'checkbox', width: '3%' },
                    { text: 'View Name', columntype: 'textbox', datafield: 'StrMessage', editable: false, width: '90%', cellsalign: 'left', renderer: columnsrenderer }
                ]
        });

        $("#jqxgridUserViews").on('bindingcomplete', function (event) {
            $("#jqxgridUserViews").jqxGrid('clearselection');
            $("#popupUserViews").unblock();
            return false;
        });

        var sourceUserViews =
            {
                url: "/Home/UserViewDataGrid",
                datatype: "json",
                datafields:
                    [
                        { name: 'MgUserId', type: 'int' },
                        { name: 'StrMessage', type: 'string' }
                    ],
                sortcolumn: 'MgUserId',
                sortdirection: 'asc',
                deleterow: function (rowid, commit) {
                    // Do something with the result
                    commit(true);
                }
            };

        var dataAdapterUV = new $.jqx.dataAdapter(sourceUserViews);

        var loadUserViewGrid = function () {
            $("#jqxgridUserViews").jqxGrid({ source: dataAdapterUV });

            $("#jqxgridUserViews").jqxGrid('clearselection');

            //Block the UI
            $("#popupUserViews").block({
                message: $('#divLoadding'),
                css: {
                    top: ($(window).height() - 400) / 2 + 'px',
                    left: ($(window).width() - 400) / 2 + 'px',
                    width: '400px'
                }
            });
        };

        var reloadUserViewGrid = function () {
            $("#jqxgridUserViews").jqxGrid('clear');
            $("#jqxgridUserViews").jqxGrid('clearselection');
            dataAdapterUV.dataBind();
            dataAdapterVw.dataBind();
            //Block the UI
            $("#popupUserViews").block({
                message: $('#divLoadding'),
                css: {
                    top: ($(window).height() - 400) / 2 + 'px',
                    left: ($(window).width() - 400) / 2 + 'px',
                    width: '400px'
                }
            });
        };

        var openUserViewDialog = function (rowindex) {
            // open the popup window when the user clicks a button.
            var offset = $("#jqxgrid").offset();
            $("#popupUserViews").jqxWindow({ position: { x: parseInt(offset.left) + 300, y: parseInt(offset.top) - 100} });
            // get the clicked row's data and initialize the input fields.
            // show the popup window.
            $("#popupUserViews").jqxWindow('open');
            loadUserViewGrid();
        };

        $("#jqxgridUserViews").on('cellendedit', function (event) {
            if (event.args.datafield == "available") {
                if (event.args.value) {
                    $("#jqxgridUserViews").jqxGrid('selectrow', event.args.rowindex);
                }
                else {
                    $("#jqxgridUserViews").jqxGrid('unselectrow', event.args.rowindex);
                }
            }
        });

        $("#closeUVWndw").jqxButton({ theme: 'arctic' });
        //Remove filter 
        $("#closeUVWndw").off('click').on('click', function () {
            $("#popupUserViews").jqxWindow('close');
        });

        $("#deleteUserView").jqxButton({ theme: 'arctic' });
        // delete quotes.
        $("#deleteUserView").off('click').on('click', function () {
            var rows = $("#jqxgridUserViews").jqxGrid('selectedrowindexes');
            var selectedRecords = new Array();
            var selectedQuoteIds = "";

            if (rows.length > 0) {
                for (var m = 0; m < rows.length; m++) {
                    var row = $("#jqxgridUserViews").jqxGrid('getrowdata', rows[m]);
                    selectedRecords[selectedRecords.length] = row;
                    selectedQuoteIds += row.MgUserId;
                    if (m < (rows.length - 1))
                        selectedQuoteIds += ",";
                }

                if (confirm("Are you sure you want to delete these?")) {
                    $.ajax({
                        url: '/Home/DeleteUserViews',
                        data: { uvList: selectedQuoteIds },
                        type: 'DELETE',
                        success: function (result) {
                            if (result.MgUserId > 0) {
                                var commit = $("#jqxgridUserViews").jqxGrid('deleterow', id);
                                reloadUserViewGrid();
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
        });

        $('#oppStatus').on('change', function (event) {
            var args = event.args;
            if (args) {
                var item = args.item;
                // get item's value.
                var value = item.value;
                if ((currStatus != value) || currChange) {
                    $.ajax
                    (
                        {
                            type: 'GET',
                            url: '/Home/LoadDefaultProbabilityByStatus',
                            data: { status: value },
                            datatype: "application/json",
                            success: function (msg) {
                                $("#probability").val(msg);
                                currChange = true
                            },
                            error: function (result) {
                                $("#probability").val(0);
                            }
                        }
                    );
                }
            }
        });

        $("#refreshbutton").jqxButton({ width: '70px', height: '23px' });
        $("#refreshbutton").off('click').on('click', function () {
            refreshGrid();
        });

        $("#exportbutton").jqxButton({ width: '70px', height: '23px' });
        // export grid.
        $("#exportbutton").off('click').on('click', function () {
            var arr = $("#jqxgrid").jqxGrid('getdisplayrows');
            for (var m = 0; m < arr.length; m++) {
                if (arr[m].OppCloseDate !== null && arr[m].OppCloseDate !== '') {
                    arr[m].OppCloseDate = new Date(arr[m].OppCloseDate);
                }
                if (arr[m].CreateDT !== null && arr[m].CreateDate !== '') {
                    arr[m].CreateDT = new Date(arr[m].CreateDT);
                }
            }
            var infoExport = $("#jqxgrid").jqxGrid('exportdata', 'csv', null, true, arr);
            infoExport = infoExport.replace(/0NaN-NaN-NaN/g, "");
            $.ajax({
                type: "POST",
                async: false,
                url: "/Home/ExportGrid",
                data: { gridData: infoExport, fileName: 'SalesManagerExport' },
                success: function (result) {
                    if (result != "") {
                        var d = new Date();
                        var n = d.getTime();
                        window.open(result + "?version=" + n, '_blank');
                    }
                },
                error: function (result, status, err) {
                    return;
                }
            });
        });

        $("#Cancel").jqxButton({ theme: 'arctic' });
        $("#Cancel").off('click').on('click', function () {
            currChange = false;
            currStatus = 0;
            $("#popupWindow").jqxWindow('hide');
        });

        $("#Save").jqxButton({ theme: 'arctic' });
        // update the edited row when the user clicks the 'Save' button.
        $("#Save").off('click').on('click', function () {
            $("#Save").jqxButton({ disabled: true });
            $("#Cancel").jqxButton({ disabled: true });
            var date = $("#closeDate").jqxDateTimeInput('getDate');
            var formattedDate = $.jqx.dataFormat.formatdate(date, 'MM/dd/yyyy');

            var dateR = $("#createDT").jqxDateTimeInput('getDate');
            var formattedDateR = "";
            if (dateR != null) {
                var formattedDateR = $.jqx.dataFormat.formatdate(dateR, 'MM/dd/yyyy');
            }

            var valid = true;

            if ($('#owner').jqxDropDownList('getSelectedItem') == null) {
                alert("Please select an owner");
                valid = false;
            }

            if ($('#oppStatus').jqxDropDownList('getSelectedItem') == null) {
                alert("Please select a status");
                valid = false;
            }

            if (valid) {
                var row = { OppID: $("#oppId").val(), CRMOppID: $("#crmOppId").val(), OppName: $("#oppName").val(), CompanyName: $("#companyName").val(),
                    OppProbability: $("#probability").val(), OppCloseDate: formattedDate, OppOwner: $('#owner').jqxDropDownList('val'), OppOwnerName: $('#owner').jqxDropDownList('getSelectedItem').label, OppStatusId: $('#oppStatus').jqxDropDownList('val'), OppStatusName: $('#oppStatus').jqxDropDownList('getSelectedItem').label,
                    QuotedAmount: getFloat($("#quotedAmtInput").val()),
                    QuotedCost: parseFloat($("#quotedCost").jqxNumberInput('decimal')), QuotedMargin: $("#quotedMargin").val(), NumofQuotes: $("#numQuotes").val(), CreateDT: formattedDateR, ClientDefinedTotal1: $("#cdfTotal1").val(),
                    ClientDefinedTotal2: $("#cdfTotal2").val(), ClientDefinedTotal3: $("#cdfTotal3").val(), ClientDefinedTotal4: $("#cdfTotal4").val(), ClientDefinedTotal5: $("#cdfTotal5").val(),
                    ClientDefinedTotal6: $("#cdfTotal6").val(), ClientDefinedTotal7: $("#cdfTotal7").val(), ClientDefinedTotal8: $("#cdfTotal8").val(), ClientDefinedTotal9: $("#cdfTotal9").val(),
                    ClientDefinedTotal10: $("#cdfTotal10").val(), ClientDefinedText1: $("#cdfText1").val(), ClientDefinedText2: $("#cdfText2").val(), ClientDefinedText3: $("#cdfText3").val(), ClientDefinedText4: $("#cdfText4").val(), ClientDefinedText5: $("#cdfText5").val()
                };

                if (editrow >= 0 && row.OppID != "") {
                    var rowId = $('#jqxgrid').jqxGrid('getrowid', editrow);
                    $.ajax({
                        url: '/Home/UpdateOpportunityRow',
                        data: row,
                        datatype: "application/json",
                        type: 'PUT',
                        success: function (result) {
                            if (result.MgUserId > 0) {
                                editrow = row.OppID;
                                var commit = $("#jqxgrid").jqxGrid('updaterow', rowId, row);
                                $("#popupWindow").jqxWindow('hide');
                            } else {
                                alert(result.StrMessage);
                            }

                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert("Error:" + XMLHttpRequest);
                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        }
                    });
                } else {
                    $.ajax({
                        url: '/Home/AddOpportunityRow',
                        data: row,
                        datatype: "application/json",
                        type: 'POST',
                        success: function (result) {
                            if (result.MgUserId > 0) {
                                var now = new Date();
                                row.CreateDT = now;
                                row.OppID = result.MgUserId;
                                row.CRMOppID = result.StrMessage;
                                var commit = $("#jqxgrid").jqxGrid('addrow', null, row);
                                $("#popupWindow").jqxWindow('hide');
                            } else {
                                alert(result.StrMessage);
                            }

                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        },
                        error: function (XMLHttpRequest, textStatus, errorThrown) {
                            alert("Error:" + XMLHttpRequest);
                            $("#Save").jqxButton({ disabled: false });
                            $("#Cancel").jqxButton({ disabled: false });
                        }
                    });
                }
            }
            else {
                $("#Save").jqxButton({ disabled: false });
                $("#Cancel").jqxButton({ disabled: false });
            }
            currChange = false;
            currStatus = 0;
        });


        var rollups = [
                 { value: "Y", label: "Y" },
                 { value: "N", label: "N" }
                 ];

        var rollupsSource = {
            datatype: "array",
            datafields: [
                     { name: 'label', type: 'string' },
                     { name: 'value', type: 'string' }
                 ],
            localdata: rollups
        };

        var rollupsAdapter = new $.jqx.dataAdapter(rollupsSource, {
            autoBind: true
        });


        var linkrenderer = function (row, column, value) {
            return "<a href='#'>" + value + "</a>";
        };

        // Initialize jqxgridQuotes
        $("#jqxgridQuotes").jqxGrid({
            width: '100%',
            height: '318px',
            editable: true,
            theme: 'arctic',
            pageable: true,
            sortable: true,
            altrows: true,
            selectionmode: 'singlecell',
            columnsresize: true,
            columnsreorder: true,
            // trigger cell hover.
            cellhover: function (element, pageX, pageY) {
                var cell = $('#jqxgridQuotes').jqxGrid('getcellatposition', pageX, pageY);
                if (cell.column == "LastFileSavedLocation") {
                    // update tooltip.
                    $("#jqxgridQuotes").jqxTooltip({ content: element.innerHTML });
                    // open tooltip.
                    $("#jqxgridQuotes").jqxTooltip('open', pageX + 15, pageY + 15);

                } else {

                    $("#jqxgridQuotes").jqxTooltip('close');
                };
            },
            ready: function () {
                getCDFOpps("Quote");
                showCDFOpps("#jqxgridQuotes");
            },
            columns:
                [
                    { text: '', datafield: 'available', columntype: 'checkbox', width: '3%' },
                    { text: 'Site Description', columntype: 'textbox', datafield: 'QuoteSiteDescription', editable: false, width: '13%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Rollup', columntype: 'dropdownlist', datafield: 'Rollup', editable: true, width: '7%', cellsalign: 'left', renderer: columnsrenderer,
                        createeditor: function (row, value, editor) {
                            editor.jqxDropDownList({ source: rollupsAdapter, displayMember: 'label', valueMember: 'value' });
                        }
                    },
                    { text: 'Total Sell', columntype: 'textbox', datafield: 'QuotedAmount', editable: false, width: '14%', cellsalign: 'right', cellsformat: 'c2', renderer: columnsrenderer },
                    { text: 'Total Cost', columntype: 'textbox', datafield: 'QuotedCost', editable: false, width: '14%', cellsalign: 'right', cellsformat: 'c2', renderer: columnsrenderer },
                    { text: 'Margin', columntype: 'textbox', datafield: 'QuotedMargin', editable: false, width: '8%', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'Last Updated By', columntype: 'textbox', datafield: 'SDALastUpdByName', editable: false, width: '15%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Last File Saved Location', columntype: 'textbox', datafield: 'LastFileSavedLocation', editable: false, width: '26%', cellsalign: 'left', renderer: columnsrenderer, cellsrenderer: linkrenderer },
                    { text: 'ClientDefinedTotal1', datafield: 'ClientDefinedTotal1', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal2', datafield: 'ClientDefinedTotal2', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal3', datafield: 'ClientDefinedTotal3', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal4', datafield: 'ClientDefinedTotal4', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal5', datafield: 'ClientDefinedTotal5', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal6', datafield: 'ClientDefinedTotal6', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal7', datafield: 'ClientDefinedTotal7', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal8', datafield: 'ClientDefinedTotal8', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal9', datafield: 'ClientDefinedTotal9', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedTotal10', datafield: 'ClientDefinedTotal10', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'ClientDefinedText1', datafield: 'ClientDefinedText1', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'ClientDefinedText2', datafield: 'ClientDefinedText2', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'ClientDefinedText3', datafield: 'ClientDefinedText3', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'ClientDefinedText4', datafield: 'ClientDefinedText4', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'ClientDefinedText5', datafield: 'ClientDefinedText5', hidden: true, columntype: 'textbox', width: '100px', cellsalign: 'left', renderer: columnsrenderer }
                ]
        });


        $("#dvData").jqxGrid({
            width: '1px',
            height: '1px',
            theme: 'arctic',
            columns:
                [
                    { text: '', datafield: 'available', hidden: true, columntype: 'checkbox', width: '3%' },
                    { text: 'Site Description', columntype: 'textbox', hidden: true, datafield: 'QuoteSiteDescription', editable: false, width: '13%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Rollup', columntype: 'dropdownlist', datafield: 'Rollup', hidden: true, editable: true, width: '7%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Total Sell', columntype: 'textbox', datafield: 'QuotedAmount', hidden: true, editable: false, width: '14%', cellsalign: 'right', cellsformat: 'c2', renderer: columnsrenderer },
                    { text: 'Total Cost', columntype: 'textbox', datafield: 'QuotedCost', hidden: true, editable: false, width: '14%', cellsalign: 'right', cellsformat: 'c2', renderer: columnsrenderer },
                    { text: 'Margin', columntype: 'textbox', datafield: 'QuotedMargin', hidden: true, editable: false, width: '8%', cellsalign: 'right', cellsformat: 'f2', renderer: columnsrenderer },
                    { text: 'Last Updated By', columntype: 'textbox', datafield: 'SDALastUpdByName', hidden: true, editable: false, width: '15%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Last File Saved Location', columntype: 'textbox', datafield: 'LastFileSavedLocation', editable: false },
                    { text: 'ClientDefinedTotal1', datafield: 'ClientDefinedTotal1', hidden: true },
                    { text: 'ClientDefinedTotal2', datafield: 'ClientDefinedTotal2', hidden: true },
                    { text: 'ClientDefinedTotal3', datafield: 'ClientDefinedTotal3', hidden: true },
                    { text: 'ClientDefinedTotal4', datafield: 'ClientDefinedTotal4', hidden: true },
                    { text: 'ClientDefinedTotal5', datafield: 'ClientDefinedTotal5', hidden: true },
                    { text: 'ClientDefinedTotal6', datafield: 'ClientDefinedTotal6', hidden: true },
                    { text: 'ClientDefinedTotal7', datafield: 'ClientDefinedTotal7', hidden: true },
                    { text: 'ClientDefinedTotal8', datafield: 'ClientDefinedTotal8', hidden: true },
                    { text: 'ClientDefinedTotal9', datafield: 'ClientDefinedTotal9', hidden: true },
                    { text: 'ClientDefinedTotal10', datafield: 'ClientDefinedTotal10', hidden: true },
                    { text: 'ClientDefinedText1', datafield: 'ClientDefinedText1', hidden: true },
                    { text: 'ClientDefinedText2', datafield: 'ClientDefinedText2', hidden: true },
                    { text: 'ClientDefinedText3', datafield: 'ClientDefinedText3', hidden: true },
                    { text: 'ClientDefinedText4', datafield: 'ClientDefinedText4', hidden: true },
                    { text: 'ClientDefinedText5', datafield: 'ClientDefinedText5', hidden: true }
                ]
        });

        $("#jqxgridQuotes").off('cellclick').on('cellclick', function (event) {
            if (event.args.datafield == 'LastFileSavedLocation' && event.args.value != null) {
                var arr = new Array();
                var row = {};
                row['LastFileSavedLocation'] = event.args.value;
                arr.push(row);

                var dataToImport = $("#dvData").jqxGrid('exportdata', 'csv', null, true, arr);

                $.ajax({
                    type: "POST",
                    url: "/Home/ExportGrid",
                    data: { gridData: dataToImport, fileName: 'OpenFile' },
                    success: function (result) {
                        if (result != "") {
                            var d = new Date();
                            var n = d.getTime();
                            window.location.href = result + "?version=" + n;
                        }
                    },
                    error: function (result, status, err) {
                        return;
                    }
                });
            }
        });

        $("#jqxgridQuotes").on('bindingcomplete', function (event) {
            $("#jqxgridQuotes").jqxGrid('clearselection');
            $("#popupQuotes").unblock();
            return false;
        });

        var loadQuotesGrid = function (oppId) {

            var sourceQuotes =
                    {
                        url: "/Home/QuoteDataGrid",
                        datatype: "json",
                        data: { opportunityID: oppId },
                        datafields:
                            [
                                { name: 'OppID', type: 'int' },
                                { name: 'QuoteID', type: 'string' },
                                { name: 'QuoteSiteDescription', type: 'string' },
                                { name: 'Rollup', type: 'string' },
                                { name: 'QuotedAmount', type: 'float' },
                                { name: 'QuotedCost', type: 'float' },
                                { name: 'QuotedMargin', type: 'float' },
                                { name: 'SDALastUpdBy', type: 'int' },
                                { name: 'SDALastUpdByName', type: 'string' },
                                { name: 'LastFileSavedLocation', type: 'string' },
            		            { name: 'available', type: 'bool' },
                                { name: 'ClientDefinedTotal1', type: 'float' },
                                { name: 'ClientDefinedTotal2', type: 'float' },
                                { name: 'ClientDefinedTotal3', type: 'float' },
                                { name: 'ClientDefinedTotal4', type: 'float' },
                                { name: 'ClientDefinedTotal5', type: 'float' },
                                { name: 'ClientDefinedTotal6', type: 'float' },
                                { name: 'ClientDefinedTotal7', type: 'float' },
                                { name: 'ClientDefinedTotal8', type: 'float' },
                                { name: 'ClientDefinedTotal9', type: 'float' },
                                { name: 'ClientDefinedTotal10', type: 'float' },
                                { name: 'ClientDefinedText1', type: 'string' },
                                { name: 'ClientDefinedText2', type: 'string' },
                                { name: 'ClientDefinedText3', type: 'string' },
                                { name: 'ClientDefinedText4', type: 'string' },
                                { name: 'ClientDefinedText5', type: 'string' }
                            ],
                        sortcolumn: 'QuoteId',
                        sortdirection: 'asc',
                        deleterow: function (rowid, commit) {
                            // Do something with the result
                            commit(true);
                        }
                    };

            var dataAdapterQuotes = new $.jqx.dataAdapter(sourceQuotes);

            $("#jqxgridQuotes").jqxGrid({ source: dataAdapterQuotes });

            $("#dvData").jqxGrid({ source: dataAdapterQuotes });


            //Block the UI
            $("#popupQuotes").block({
                message: $('#divLoadding'),
                css: {
                    top: ($(window).height() - 400) / 2 + 'px',
                    left: ($(window).width() - 400) / 2 + 'px',
                    width: '400px'
                }
            });
        };

        var reloadQuotesGrid = function () {
            $("#jqxgridQuotes").jqxGrid('clear');
            $("#jqxgridQuotes").jqxGrid('clearselection');
            dataAdapterQuotes.dataBind();
            //Block the UI
            $("#popupQuotes").block({
                message: $('#divLoadding'),
                css: {
                    top: ($(window).height() - 400) / 2 + 'px',
                    left: ($(window).width() - 400) / 2 + 'px',
                    width: '400px'
                }
            });
        };

        //***************************************************************//
        // Part for the Quote dialog
        //***************************************************************//
        var openQuoteDialog = function (rowindex) {
            var dataRecord = $("#jqxgrid").jqxGrid('getrowdata', rowindex);
            // open the popup window when the user clicks a button.
            var offset = $("#jqxgrid").offset();
            $("#popupQuotes").jqxWindow({ position: { x: parseInt(offset.left) + 150, y: parseInt(offset.top) - 100} });
            // get the clicked row's data and initialize the input fields.
            dataRecord = $("#jqxgrid").jqxGrid('getrowdata', rowindex);
            // show the popup window.
            $("#popupQuotes").jqxWindow('open');

            $("#popupTitle").text('Quotes for ' + dataRecord.OppName + ' (Opportunity ID ' + dataRecord.CRMOppID + ') for ' + dataRecord.CompanyName);

            var oppId = dataRecord.OppID;
            loadQuotesGrid(oppId);
        };


        $("#jqxgrid").off('cellclick').on('cellclick', function (event) {
            if (event.args.datafield == 'NumofQuotes' && event.args.value != null) {
                openQuoteDialog(event.args.rowindex);
            }

            if ((event.args.datafield == 'CRMOppID' || event.args.datafield == 'OppName') && event.args.value != null) {
                editOpportunity(event.args.rowindex);
            }
        });

        $("#jqxgridQuotes").on('cellendedit', function (event) {
            if (event.args.datafield == "Rollup") {
                var column = $("#jqxgridQuotes").jqxGrid('getcolumn', event.args.datafield);
                var datarow = $("#jqxgridQuotes").jqxGrid('getrowdata', event.args.rowindex);

                $.ajax({
                    url: '/Home/UpdateQuoteRollup',
                    data: {
                        opportunityID: datarow.OppID,
                        quoteID: datarow.QuoteID,
                        rollup: event.args.value
                    },
                    type: 'PUT',
                    success: function (result) {
                        reloadGrid();
                    },
                    error: function (result) {
                        alert(result);
                    }
                });
            }
            if (event.args.datafield == "available") {
                if (event.args.value) {
                    $("#jqxgridQuotes").jqxGrid('selectrow', event.args.rowindex);
                }
                else {
                    $("#jqxgridQuotes").jqxGrid('unselectrow', event.args.rowindex);
                }
            }
        });

        $("#closeQuoteWndw").jqxButton({ theme: 'arctic' });
        //Remove filter 
        $("#closeQuoteWndw").off('click').on('click', function () {
            $("#popupQuotes").jqxWindow('close');
        });

        $("#reassignOpportunity").jqxButton({ theme: 'arctic' });
        //Reassign 
        $("#reassignOpportunity").off('click').on('click', function () {
            openReassignDialog();
        });

        $("#deleteQuote").jqxButton({ theme: 'arctic' });
        // delete quotes.
        $("#deleteQuote").off('click').on('click', function () {
            var rows = $("#jqxgridQuotes").jqxGrid('selectedrowindexes');
            var selectedRecords = new Array();
            var selectedQuoteIds = "";

            if (rows.length > 0) {
                for (var m = 0; m < rows.length; m++) {
                    var row = $("#jqxgridQuotes").jqxGrid('getrowdata', rows[m]);
                    selectedRecords[selectedRecords.length] = row;
                    selectedQuoteIds += row.QuoteID;
                    if (m < (rows.length - 1))
                        selectedQuoteIds += ",";
                }

                if (confirm("Are you sure you want to delete these?")) {
                    $.ajax({
                        url: '/Home/DeleteQuotes',
                        data: { quoteIdList: selectedQuoteIds },
                        type: 'DELETE',
                        success: function (result) {
                            if (result.MgUserId > 0) {
                                var commit = $("#jqxgridQuotes").jqxGrid('deleterow', id);
                                reloadQuotesGrid();
                                reloadGrid();
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
        });

        //***************************************************************//
        // Part for the Reassign dialog
        //***************************************************************//
        // initialize the popup window and buttons.
        var openReassignDialog = function () {
            var rows = $("#jqxgridQuotes").jqxGrid('selectedrowindexes');

            if (rows.length > 0) {
                // open the popup window when the user clicks a button.
                var offset = $("#jqxgrid").offset();
                $("#popupReassignQuotes").jqxWindow({ position: { x: parseInt(offset.left) + 250, y: parseInt(offset.top) - 90} });
                // show the popup window.
                $("#popupReassignQuotes").jqxWindow('open');

                reloadReassignOppGrid();
            }
            else {
                alert("You must select the quotes.");
            }
        };

        // Initialize jqxgridQuotes
        $("#jqxgridReassignOpp").jqxGrid({
            width: '100%',
            height: '318px',

            editable: false,
            theme: 'arctic',
            pageable: true,
            filterable: true,
            autoshowfiltericon: true,
            sortable: true,
            altrows: true,
            selectionmode: 'singlerow',
            columnsresize: true,
            columnsreorder: true,
            columns:
                [
                    { text: 'Opportunity ID', columntype: 'textbox', datafield: 'CRMOppID', width: '20%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Opportunity Name', columntype: 'textbox', datafield: 'OppName', width: '20%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Company Name', columntype: 'textbox', datafield: 'CompanyName', width: '20%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Opp Status', columntype: 'textbox', datafield: 'OppStatusName', width: '20%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'Opp Owner', columntype: 'textbox', datafield: 'OppOwnerName', width: '20%', cellsalign: 'left', renderer: columnsrenderer },
                    { text: 'OppID', datafield: 'OppID', hidden: true },
                    { text: 'OppStatusId', datafield: 'OppStatusId', hidden: true },
                    { text: 'OppOwner', datafield: 'OppOwner', hidden: true }
                ]
        });

        var reloadReassignOppGrid = function () {

            var sourceReassign =
                    {
                        url: "/Home/ReassignOpportunityDataGrid",
                        datatype: "json",
                        datafields:
                            [
                                { name: 'OppID', type: 'int' },
                                { name: 'OppStatusId', type: 'int' },
                                { name: 'OppStatusName', type: 'string' },
                                { name: 'OppProbability', type: 'int' },
                                { name: 'OppCloseDate', type: 'datetime' },
                                { name: 'OppOwner', type: 'string' },
                                { name: 'OppOwnerName', type: 'string' },
                                { name: 'OppName', type: 'string' },
                                { name: 'CompanyName', type: 'string' },
                                { name: 'CRMOppID', type: 'string' },
                                { name: 'QuotedAmount', type: 'float' },
                                { name: 'QuotedCost', type: 'float' },
                                { name: 'QuotedMargin', type: 'float' },
                                { name: 'NumofQuotes', type: 'int' },
                                { name: 'Manager', type: 'string' }
                            ],
                        sortcolumn: 'OppID',
                        sortdirection: 'asc'
                    };

            var dataAdapterReassign = new $.jqx.dataAdapter(sourceReassign);
            $("#jqxgridReassignOpp").jqxGrid({ source: dataAdapterReassign });
        };

        $("#removeFilter").jqxButton({ theme: 'arctic' });
        //Remove filter 
        $("#removeFilter").off('click').on('click', function () {
            $("#jqxgridReassignOpp").jqxGrid('clearfilters');
        });

        $("#closeWindow").jqxButton({ theme: 'arctic' });
        //Remove filter 
        $("#closeWindow").off('click').on('click', function () {
            $("#popupReassignQuotes").jqxWindow('close');
        });

        $("#reassign").jqxButton({ theme: 'arctic' });
        // Reassign quotes.
        $("#reassign").off('click').on('click', function () {
            var rows = $("#jqxgridQuotes").jqxGrid('selectedrowindexes');
            var selectedRecords = new Array();
            var selectedQuoteIds = "";

            var row = $("#jqxgridReassignOpp").jqxGrid('getselectedrowindex');
            var rowscount = $("#jqxgridReassignOpp").jqxGrid('getdatainformation').rowscount;
            var id = $("#jqxgridReassignOpp").jqxGrid('getrowid', row);
            var datarow = $("#jqxgridReassignOpp").jqxGrid('getrowdata', row);

            if (rows.length > 0) {
                for (var m = 0; m < rows.length; m++) {
                    var quoteRow = $("#jqxgridQuotes").jqxGrid('getrowdata', rows[m]);
                    selectedRecords[selectedRecords.length] = quoteRow;
                    selectedQuoteIds += quoteRow.QuoteID;
                    if (m < (rows.length - 1))
                        selectedQuoteIds += ",";
                }

                if (row != -1) {
                    $.ajax({
                        url: '/Home/ReassignQuotes',
                        data: {
                            quoteIdList: selectedQuoteIds,
                            newOpportunityID: datarow.OppID,
                            newCRMOppID: datarow.CRMOppID
                        },
                        type: 'PUT',
                        success: function (result) {
                            if (result.MgUserId > 0) {
                                reloadQuotesGrid();
                                reloadGrid();
                                $("#popupReassignQuotes").jqxWindow('hide');
                            }
                            else {
                                alert(result.StrMessage);
                            }
                        },
                        error: function (result) {
                            alert(result);
                        }
                    });
                }
                else {
                    alert("You must select an opportunity.");
                }
            }
        });

        var isLoggedOff = false;


        $("#logoffLink").off('click').on('click', function () {
            isLoggedOff = true;
        });

        window.onbeforeunload = function () {
            if (isLoggedOff) {
                sessvars.$.clearMem();
                isLoggedOff = false;
            }
            else {
                var a = $("#jqxgrid").jqxGrid('getboundrows');
                for (var m = 0; m < a.length; m++) {
                    if (a[m].OppCloseDate !== null) {
                        a[m].OppCloseDate = Date.parse(a[m].OppCloseDate);
                    }
                    if (a[m].CreateDT !== null) {
                        a[m].CreateDT = Date.parse(a[m].CreateDT);
                    }
                }
                sessvars.records = a;
                var getState = $("#jqxgrid").jqxGrid('savestate');
                sessvars.state = getState;
                sessvars.customFilter = strCustomFilter;
                sessvars.viewTitle = $("#viewTitle").text();
            }
        };
    });
} ());