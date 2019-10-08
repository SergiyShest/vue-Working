Vue.component('error-aborder',
    {
        props: {
            error: String
        },
        template:
            '<div class="tooltip" >' +
                '<div v-bind:class="{error:!error==\'\' }" >' +
                '<slot>test</slot>' +
                '</div>' +
                '<p  class="tooltiptext"  v-if="!error==\'\'" >{{error}}</p>' +
                '</div>'
    });