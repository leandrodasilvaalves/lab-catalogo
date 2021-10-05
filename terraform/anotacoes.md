## primeiro comando. executar quando alterar alguma dependencia
```
terraform init
```

## verificar o que vai ser executado
```
terraform plan -var "do_token=${DO_PAT}"
```

## criar a infra estrutura
```
terraform apply -var "do_token=${DO_PAT}" 
```

## destruir a infra apos a conclusao do estudo
```
terraform destroy -var "do_token=${DO_PAT}"
```