var baseMixin = {
    data: function () {
        return {
            abortController: new AbortController(),
            validationCounter: 0,
            ActionPath: ""
        }
    },
    methods: {
        fetchJson(path, collback, signal) {
            try {
                fetch(path, { mode: 'cors', signal: signal })
                    .then(response => response.json())
                    .then(function (json) {
                        collback(json);
                    }
                    );
            } catch (ex) {
                alert(ex);
            }
        },
        //Валидация с задержкой и прерыванием предыдущей валидации, если последущая валидация не отработала
        Validate() {
            this.validationCounter++;//увеличиваю счетчик валидации, что бы предыдущая валидация не отработала
            var counter = this.validationCounter;//создаю копию переменнной
            var delayedValidateRef = this.delayedValidate;//так как я не могу использовать this локальной фнукции создавать ссылку на функцию
            setTimeout(function () { delayedValidateRef(counter); }, 500);//вызываю валидацию с задержкой ()


        },
        delayedValidate(сounter) {
            if (this.validationCounter !== сounter)//если счетчик валидаций изменился за время задержки валидации, то эту валидацию надо прервать
            {
                return;
            }
            console.log("Validate");
            this.abortController.abort();//прервать предыдущую валидацию если она успела начаться
            this.abortController = new AbortController();//создаю новый контроллер валидации
            this.Action("Validate", this.abortController.signal);//перадаю сигнал позволяющий "убить" ПРЕДЫДУЩИЙ фетч
        },
        Save() {
            this.abortController.abort();//прервать предыдущую валидацию
            this.Action("Save");
        },
        Action(action, signal) {
            var myJSON = JSON.stringify(this.order);
            var path = this.ActionPath + action + "?id=" + this.id + "&json=" + myJSON;
            this.fetchJson(path, jsonResult => {
                this.errors = jsonResult.Errors;
                this.IsChanged = jsonResult.IsChanged;
            }, signal);
        },
    }
}