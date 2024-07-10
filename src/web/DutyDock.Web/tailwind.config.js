/** @type {import('tailwindcss').Config} */
module.exports = {
    content: [
        './**/*.{razor,html,razor.cs}',
        '../DutyDock.Web.Components/**/*.{razor,html,razor.cs}'
    ],
    corePlugins: {
        container: false
    },
    safelist: [
        'cursor-pointer'
    ],
    important: true,
    theme: {
        extend: {
            fontFamily: {
                jost: ['Jost', 'sans-serif'],
                albert: ['Albert Sans', 'sans-serif']
            },
            colors: {
                'iz-blue': {
                    '50': '#f2f7fd',
                    '100': '#e5eef9',
                    '200': '#c4dcf3',
                    '300': '#91c0e8',
                    '400': '#56a0da',
                    '500': '#3083c7',
                    '600': '#2167a8',
                    '700': '#1c5388',
                    '800': '#1b4771',
                    '900': '#1b3c5f',
                    '950': '#12273f'
                },
                'iz-cyan': {
                    '50': '#f2fbf9',
                    '100': '#d2f5ef',
                    '200': '#a5eadf',
                    '300': '#70d8cb',
                    '400': '#3baea3',
                    '500': '#29a39a',
                    '600': '#1e837d',
                    '700': '#1c6965',
                    '800': '#1b5453',
                    '900': '#1a4745',
                    '950': '#092a2a',
                },
                'iz-grey': {
                    '50': '#f8fafa',
                    '100': '#f2f4f5',
                    '200': '#e7ebed',
                    '300': '#d4dbde',
                    '400': '#bbc4ca',
                    '500': '#9eabb4',
                    '600': '#8794a0',
                    '700': '#74808d',
                    '800': '#616c76',
                    '900': '#515961',
                    '950': '#353b40',
                },
                'iz-green': {
                    '50': '#f1fcf5',
                    '100': '#e0f8e9',
                    '200': '#c2f0d4',
                    '300': '#92e3b3',
                    '400': '#5bcd8a',
                    '500': '#39c171',
                    '600': '#269353',
                    '700': '#217444',
                    '800': '#1f5c39',
                    '900': '#1b4c30',
                    '950': '#0a2918',
                },
                'iz-red': {
                    '50': '#fef2f2',
                    '100': '#fee2e2',
                    '200': '#fdcbcb',
                    '300': '#fba6a6',
                    '400': '#f67373',
                    '500': '#ed4646',
                    '600': '#da2828',
                    '700': '#b71e1e',
                    '800': '#981c1c',
                    '900': '#7e1e1e',
                    '950': '#440b0b',
                },
                'iz-yellow': {
                    '50': '#fef9ec',
                    '100': '#fbeeca',
                    '200': '#f7db90',
                    '300': '#f3c456',
                    '400': '#f0ad2f',
                    '500': '#e98d17',
                    '600': '#ce6911',
                    '700': '#ab4a12',
                    '800': '#8b3a15',
                    '900': '#733014',
                    '950': '#421706',
                }
            }
        },
    },
    plugins: [],
}

