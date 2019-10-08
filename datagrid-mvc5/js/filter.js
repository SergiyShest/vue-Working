var dataGrid;

//объект который 
function FilterField(dataGridColumn, input, checkBox, condition = '=') {
    this.column = dataGridColumn;
    this.DataField = dataGridColumn.dataField;
    this.DataType = dataGridColumn.dataType;
    this.Condition = condition;
    this.Input = input;
    this.Input.Tag = this;
    this.Input.onkeyup = ValueChanged;
    this.CheckBox = checkBox;
    this.CheckBox.Tag = this;
    this.ApplayFilter = ApplayFilter;//function применить фильтр
    this.ChangeChecked  = ChangeChecked;//function изменилась галочка
    function ApplayFilter(collectiveFilter) {
        if (checkBox.checked) {
            filter = new Array();
            var val = this.Input.value;
            switch (this.DataType) {
                case 'number': {
                    if (val.includes(',')) {
                        filter.push(filterCommaSplittedCodition(this.DataField, val));
                    }
                    else { filter.push([this.DataField, '=', val]); }
                } break;
                case 'string': { filter.push([this.DataField, 'contains', val]); }
            }
            collectiveFilter.push(filter);
        }
    }
   //разделение по значения фильтра по запятым
    function filterCommaSplittedCodition(dataField, inpVal) {
        inpVal = inpVal.replace(/\s/g, '').replace(/,+/g, ',');//remove repeated commas and spaces
        var arr = inpVal.split(',');
        filter = new Array();
        if (arr.length == 1 && arr[0] == '') { } else {
            for (f = 0, i = 0; i < arr.length; i++ , f++) {
                filter[f] = [dataField, "=", arr[i]];
                if (arr.length - 1 > i) {
                    f++;
                    filter[f] = 'or'
                }
            }
        }
        return filter;
    }
//called from OUTside 
//change CheckBox value depending on the InputBox.value  
    function ChangeChecked() {
        if (this.Input.value === "") {
            this.CheckBox.checked = false;
        } else {
            this.CheckBox.checked = true;
        }
    }
//called from InputBox 
//change CheckBox value depending on the InputBox.value 
    function ValueChanged(e) {
        if (this.value === "") {
            this.Tag.CheckBox.checked = false;
        } else {
            this.Tag.CheckBox.checked = true;
        }
    }

}




var FilterElementsArray = new Array();


function CreateFilter(id,columns) {
    var filtElem = document.getElementById(id);
    filtElem.childNodes.length = 0;
    var dataGridInstance = $("#grid").dxDataGrid("instance");
    oldGridFilter = dataGridInstance.getCombinedFilter();

    const table = document.createElement('table');
    filtElem.appendChild(table);

     for (var i = 0; i < columns.length; i++) {
    var column=columns[i];

    if(!column.filter){
         continue;
       }
        row = document.createElement('tr');
        table.appendChild(row);

      
      var text=column.caption;
      if(!text)
      {
       text= column.dataField;
      }
        //label
        textnode = document.createTextNode(text);

        //create  checkbox
        checkBox = document.createElement("input");
        checkBox.setAttribute('type', 'checkbox');

        //input
        input = document.createElement("input")
      
        
        if(column.filterType && column.filterType=='ComboBox')
        {
            input = document.createElement("select")
    
         }
        field = new FilterField(column, input, checkBox);

        FilterElementsArray.push(field);
        createTableСell(row, textnode);
        createTableСell(row, checkBox);
        createTableСell(row, input);
        //           FilterElementsArray.push({ DataField: column.dataField, DataType: column.dataType, Input: input, Check: checkBox });
    }
    row = document.createElement('tr');

    table.appendChild(row);
    findButton = document.createElement("button");
    findButton.setAttribute('onclick', 'FilterFind()');
    findButton.setAttribute('class', 'float-right');
    findButton.textContent = 'Найти';
    createTableСell(row);// create 2 empty cell
    createTableСell(row);
    createTableСell(row, findButton);//put button in 3 cell

    oldGridFilter = dataGridInstance.getCombinedFilter();
    if (oldGridFilter != undefined) {
        SetOldFilter(oldGridFilter);
    }
    function createTableСell(row, el) {
        td = document.createElement("td");
        if (el != null) { td.appendChild(el); }
        row.appendChild(td);
        return td;
    }
}

function SetOldFilter(oldGridFilter) {
    if (oldGridFilter === undefined) return;

    const first = oldGridFilter[0];
    const condition = oldGridFilter[1];
    const val = oldGridFilter[2];
    if (typeof first === 'string') {
        var ff = FindByDateField(first);
        if (ff != undefined) {
            ff.Input.Condition = condition;
            if (ff.Input.value == "") { ff.Input.value = val; }
            else {
                ff.Input.value += ',' + val;
            }
            ff.ChangeChecked();
        } else {
            console.error("не найдено свойство " + first);
        }
    }
    else {
        oldGridFilter.forEach(element => {
            if (typeof element != 'string') { SetOldFilter(element); }
        });


    }
}

function ClearFilter() {
    for (i = 0; i < FilterElementsArray.length; i++) {
        FilterElementsArray[i].Input.value = "";
        FilterElementsArray[i].ChangeChecked();
    }
}
function FindByDateField(dataField) {
    for (i = 0; i < FilterElementsArray.length; i++) {
        if (FilterElementsArray[i].DataField == dataField) { return FilterElementsArray[i] }
    }
}
//Find by Filter
function FilterFind() {
    collectiveFilter = new Array();
    var dataGrid = $("#grid").dxDataGrid("instance");
    for (i = 0; i < FilterElementsArray.length; i++) {

        FilterElementsArray[i].ApplayFilter(collectiveFilter);


    }
    if (collectiveFilter.length > 0) {
        resultFilter = new Array();
        for (i = 0; i < collectiveFilter.length; i++) {
            resultFilter.push(collectiveFilter[i]);
            if (collectiveFilter.length > i + 1) {
                resultFilter.push('and');
            }

        }
        dataGrid.filter(resultFilter);
    }
    dataGrid.refresh();
    console.time("x");
}

