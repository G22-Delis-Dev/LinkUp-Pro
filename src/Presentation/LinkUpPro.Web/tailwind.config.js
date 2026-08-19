/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./Views/**/*.cshtml",
    "./wwwroot/js/**/*.js"
  ],
  theme: {
    extend: {
      colors: {
        primary: "#0A0E17",
        secondary: "#7C3AED",
        accent: "#00D4FF",
        "card-bg": "rgba(255, 255, 255, 0.03)",
        "card-border": "rgba(255, 255, 255, 0.1)",
        "text-primary": "#F8FAFC",
        "text-muted": "#94A3B8"
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif']
      }
    },
  },
  plugins: [],
}
