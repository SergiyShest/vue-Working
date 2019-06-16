Vue.component('error-aborder',
    {
        props: {
            error: String
        },
        data: function () {
            return {
                count: 0
            }
        },
        template:
            '<div class="tooltip" >' +
                '<div v-bind:class="{error:!error==\'\' }" >' +
                '<slot>test</slot>' +
                '</div>' +
                '<p  class="tooltiptext"  v-if="!error==\'\'" >{{error}}</p>' +
                '</div>'
    });