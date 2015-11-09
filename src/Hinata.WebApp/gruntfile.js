module.exports = function (grunt) {
    grunt.initConfig({
        bower: {
            install: {
                options: {
                    targetDir: "",
                    layout : function (type, component) {
                        if (type === 'css') {
                            return 'Content/' + component;
                        } else {
                            return 'Scripts/' + component;
                        }
                    },
                    cleanTargetDir: false
                }
            }
        }
    });

    grunt.registerTask("default", ["bower:install"]);
    grunt.loadNpmTasks("grunt-bower-task");
}