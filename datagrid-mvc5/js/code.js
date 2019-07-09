function copyToClipboard (str ) {
    const el = document.createElement('textarea');
    el.value = str;
    document.body.appendChild(el);
    el.select();
    document.execCommand('copy');
    document.body.removeChild(el);
};
window.addEventListener('resize',AutoSizeDataGrid );

function AutoSizeDataGrid() {

    var dataGridEl = = $('#grid');
    console.log(dataGridEl);
    var grid = $("#grid").dxDataGrid("instance");
    grid.option("height", dataSource);
  //  document.querySelector('.width').innerText = document.documentElement.clientWidth;
  //  document.querySelector('.height').innerText = document.documentElement.clientHeight;
}
var statuses = ["All", 'Switzerland', 'Belgium', 'Brazil', 'France', 'Brazil', 'Germany',];
//добавление пункта меню копировать
function contextMenuPreparing(e) {
    if (e.target == 'header') {
        var dataGrid = $('#grid').dxDataGrid('instance');
        var selectedRows = dataGrid.getSelectedRowsData();
        if (selectedRows.length > 0) {
            var ind = e.column.dataField;
            e.items.push({
                text: "Copy",
                onItemClick: function () {
                    var res = '';
                    for (i = 0; i < selectedRows.length; i++) {
                        res += selectedRows[i][ind];
                        if (i < electedRows.length + 1) { res += ',' }
                    }
                    copyToClipboard(res.trim(','));
                }
            });
        }
    }
}

$(function () {

//получение строки фильтров
function processFilter(dataGridInstance, filter) {
    if ($.isArray(filter)) {
        if ($.isFunction(filter[0])) {
            filter[0] = getColumnFieldName(dataGridInstance, filter[0]);
        }
        else {
            for (var i = 0; i < filter.length; i++) {
                processFilter(dataGridInstance, filter[i]);
            }
        }
    }
}

$("#buttonContainer").dxButton({
    text: "проверка фильтра",
    onClick: function (e) {
        var grid = $("#grid").dxDataGrid("instance"),
        filter = grid.getCombinedFilter();
        processFilter(grid, filter);
        alert(filter);
    }
}).dxButton("instance");
//поиск имени конолки по фильтру
function getColumnFieldName(dataGridInstance, getter) {
    var column,
        i;

    if ($.isFunction(getter)) {
        for (i = 0; i < dataGridInstance.columnCount(); i++) {
            column = dataGridInstance.columnOption(i);
            if (column.calculateCellValue.guid === getter.guid) {
                return column.dataField;
            }
        }
    }
    else {
        return getter;
    }
}


    $("#grid").dxDataGrid({
        height: 500,
        remoteOperations: { paging: true, filtering: true, sorting: true, grouping: true, summary: true, groupPaging: true },

        dataSource: DevExpress.data.AspNet.createStore({
            key: "OrderID",
            loadUrl: "../Orders/Get",
            insertUrl: "../Orders/Post",
            updateUrl: "../Orders/Put",
            deleteUrl: "../Orders/Delete"
        }),
        paging: {
            pageSize: 30
        },
        onContextMenuPreparing: contextMenuPreparing,
        focusedRowEnabled: true,
        rowAlternationEnabled: true,
        focusedRowKey: 3,
        // filterValue: [["OrderID", ">", "400"], "and", ["OrderID", '<',"100000"]],
        onInitNewRow: function (e) {
            e.data = {
                OrderDate: new Date()
            };
        },
        columnAutoWidth: true,
        filterRow: {
            visible: true,
            applyFilter: "auto"
        },

        headerFilter: { visible: true },
        filterPanel: { visible: true },
        selection: {
            mode: "multiple",
            allowSelectAll: true
        },
        columns: [
            {
                dataField: "OrderID",
                formItem: {
                    visible: true
                },
                headerCellTemplate: $('<i style="color: black">ID</i>')
            },
            {
                caption: "Customer",
                calculateDisplayValue: "CustomerName",
                dataField: "CustomerID",
                lookup: {
                    valueExpr: "CustomerID",
                    displayExpr: "ContactName",
                    dataSource: {
                        paginate: true,
                        store: DevExpress.data.AspNet.createStore({
                            key: "CustomerID",
                            loadUrl: "Customers/Get"
                        })
                    }
                }
            },
            {
                caption: "Employee",
                calculateDisplayValue: "EmployeeName",
                dataField: "EmployeeID",
                lookup: {
                    valueExpr: "EmployeeID",
                    displayExpr: "FullName",
                    dataSource: {
                        paginate: true,
                        store: DevExpress.data.AspNet.createStore({
                            key: "EmployeeID",
                            loadUrl: "Employees/Get"
                        })
                    }
                }
            },
            { caption: "дата ",dataField: "OrderDate", dataType: "date" },
            { dataField: "RequiredDate", dataType: "date" },
            { dataField: "ShippedDate", dataType: "date" },
            {
                dataField: "ShipVia",
                calculateDisplayValue: "ShipViaName",
                lookup: {
                    valueExpr: "ShipperID",
                    displayExpr: "CompanyName",
                    dataSource: {
                        paginate: true,
                        store: DevExpress.data.AspNet.createStore({
                            key: "ShipperID",
                            loadUrl: "Shippers/Get"
                        })
                    }
                }
            },
            "Freight",
            "ShipName",
            "ShipAddress",
            "ShipCity",
            "ShipCountry"
        ]
    });

    function masterDetailTemplate(container, options) {
        $("<div>").addClass("grid").appendTo(container).dxDataGrid({
            remoteOperations: true,
            dataSource: {
                filter: ["OrderID", "=", options.key],
                store: DevExpress.data.AspNet.createStore({
                    key: ["OrderID", "ProductID"],
                    loadUrl: "OrderDetails/Get",
                    insertUrl: "OrderDetails/Post",
                    updateUrl: "OrderDetails/Put",
                    deleteUrl: "OrderDetails/Delete",
                })
            },
            showBorders: true,
            editing: {
                allowUpdating: true,
                allowAdding: true,
                allowDeleting: true
            },
            onInitNewRow: function (e) {
                e.data = {
                    OrderID: options.key,
                    Quantity: 1,
                    Discount: 0
                }
            },
            onEditorPreparing: function (e) {
                if (e.dataField === "ProductID") {
                    var dataGrid = e.component;
                    var valueChanged = e.editorOptions.onValueChanged;
                    e.editorOptions.onValueChanged = function (args) {
                        valueChanged.apply(this, arguments);

                        var product = args.component.getDataSource().items().filter(function (data) { return data.ProductID === args.value })[0];

                        if (product) {
                            dataGrid.cellValue(e.row.rowIndex, "UnitPrice", product.UnitPrice);
                        }
                    }
                }
            },
            summary: {
                totalItems: [
                    { column: "Total", summaryType: "sum", displayFormat: "Total: {0}", valueFormat: { type: "currency", precision: 2 } }
                ]
            },
            columns: [
                {
                    dataField: "ProductID",
                    caption: "Product",
                    calculateDisplayValue: "ProductName",
                    lookup: {
                        valueExpr: "ProductID",
                        displayExpr: "ProductName",
                        dataSource: {
                            paginate: true,
                            store: DevExpress.data.AspNet.createStore({
                                key: "ProductID",
                                loadUrl: "Products/Get"
                            })
                        }
                    }
                },
                { dataField: "UnitPrice", format: { type: "currency", precision: 2 }, allowEditing: false },
                "Quantity",
                "Discount",
                { dataField: "Total", format: { type: "currency", precision: 2 }, allowEditing: false, calculateCellValue: function (data) { return data.UnitPrice ? data.UnitPrice * data.Quantity * (1 - data.Discount) : null } }
            ]
        })
    }


});