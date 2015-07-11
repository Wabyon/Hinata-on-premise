module.exports = function (grunt) {
    grunt.initConfig({
        bower: {
            install: {
                options: {
                    targetDir: "./Scripts",
                    layout: "byType",
                    cleanTargetDir: false
                }
            }
        }
    });

    grunt.registerTask("default", ["bower:install"]);
    grunt.loadNpmTasks("grunt-bower-task");
}