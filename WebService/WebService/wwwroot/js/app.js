  $(document).ready(function(){
            // Shows/Hides SideNav
            $('.button-collapse').sideNav({
                menuWidth: 240,
                closeOnClick: true // Closes side-nav on <a> clicks, useful for Angular/Meteor
            });
            $('.collapsible').collapsible();

            // Shows Modal 
            $('.modal').modal();

            // Js variation of datepicker.js
            $('.datepicker').pickadate({
                selectMonths: true,
                selectYears: 15,
                today: 'Vandaag',
                clear: 'Wis',
                close: 'Akkoord',
                closeOnSelect: false
            });

        });