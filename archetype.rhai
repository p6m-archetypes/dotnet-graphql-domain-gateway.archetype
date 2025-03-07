let context = #{};

if switch_enabled("debug-answers") {
    log(Info, archetect::archetype::description() +" Answers:");
    display(as_yaml(ANSWERS));
}

// Use the same switches, defaults, etc passed to this archetype, and pass them to the next
let render_settings = archetect::archetype::settings();

context += render(Archetype("org-prompts"), ANSWERS, render_settings);

context += render(Archetype("java-project-attributes"), ANSWERS + context + #{
    "project-suffix": "Domain Gateway",
}, render_settings);

context += prompt("Artifactory Host:", "artifactory-host", #{
  defaults_with: "p6m.jfrog.io",
});

context += set("service-port", 9000);
context += set("management-port", context["service-port"] + 1);
context += set("debug-port", context["service-port"] + 9);
context += set("project-language", ".NET");

if ANSWERS.services != () {
    context.services = ANSWERS.services;
} else {
    context.services = default_services(context);
}

if switch_enabled("debug-context") {
    log(Info, `${archetect::archetype::description()} (${context["project-name"]}) Context:`);
    display(as_yaml(context));
}

render(Directory("contents/base"), context);

for service in context.services.values() {
  for entity_key in service.model.entities.keys() {
    let entity = service.model.entities[entity_key];
    let entity_context = #{};
    entity_context += set("entity-name", entity_key, #{
        cases: [
            CasedIdentityCasedValue(PROGRAMMING_CASES),
       ],
    });
    let render_context = context;
    render_context += entity_context;
    render_context.service = service;
    render(Directory("contents/entity"), render_context);
  }
}

render(Archetype("manifests"), context["artifact-id"], context + #{ "project-suffix": "Domain Gateway"});
//render(Archetype("gitignore"), context["artifact-id"], #{ "ignore": ["java"]});

fn default_services(context) {
    let yaml = `
---
${context["project-prefix"]}:
  ProjectName: ${context["ProjectPrefix"]}Service
  project-name: ${context["project-prefix"]}-service
  service-port: 5030
  model:
    entities:
      ${context["project-prefix"]}:
        fields:
          id:
            optional: false
            type: String
          name:
            optional: false
            type: String
`;
    let model = from_yaml(yaml);
    model
}


// Return Context
context
